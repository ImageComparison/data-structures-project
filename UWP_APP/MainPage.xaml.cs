using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_APP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string query_directory = "";
        private string[] ref_directories = { };
        private string[] ref_names = { };
        private byte[][] ref_raw_data; //[ref_item][byte]
        private uint[] ref_widths = { };
        private uint[] ref_heights = { };
        int[][] ref_barcodes;
        private byte[] query_raw_data;
        private int query_width;
        private int query_height;
        int[] query_barcode;

        public MainPage()
        {
            this.InitializeComponent();
            
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            if (NavigationViewControl.IsBackButtonVisible.Equals(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed))
            {
                minimalIndent = 48;
            }

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

        public async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                ImageProperties properties = await file.Properties.GetImagePropertiesAsync(); //get image height and width
                uint image_width = properties.Width;
                uint image_height = properties.Height;
                query_width = (int)image_width;
                query_height = (int)image_height;

                query_directory = file.Path; //save file path
                WriteableBitmap bitmap = new WriteableBitmap(64, 64); //create WriteableBitmap with correct pix sizes

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read)) //get byte array from WriteableBitmap
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    bitmap.SetSource(stream);

                    //Scale image
                    BitmapTransform transform = new BitmapTransform()
                    {
                        ScaledWidth = image_width,
                        ScaledHeight = image_height
                    };

                    PixelDataProvider pixeldata = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage
                    );

                    query_raw_data = pixeldata.DetachPixelData(); //get raw img data
                }

                img_query.Source = bitmap; //Update query image display on ui
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                foreach (Windows.Storage.StorageFile file in files)
                {
                    ImageProperties properties = await file.Properties.GetImagePropertiesAsync(); //get image height and width
                    ref_directories.Append(file.Path); //add file directories to array
                    ref_names.Append(file.Name); //add file names to array
                    uint image_width = properties.Width;
                    uint image_height = properties.Height;
                    ref_widths.Append(image_width);
                    ref_heights.Append(image_height);

                    //TODO: Update list on side to include new items
                    //NavigationViewControl.MenuItems.Add();
                    //NavigationViewControl.MenuItems.RemoveAt();
                }
            }
        }

        private async void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                ref_directories.Append(folder.Path);
                ref_names.Append(folder.Name);

                //TODO: Update list on side to include new folder

                /*
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                */
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Remove items from list and from ref_directories and ref_names
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            query_barcode = QueryImage.Generate_Barcode(query_raw_data, query_width, query_height);

            ref_barcodes = null;
            ref_heights = null;
            ref_widths = null;
            ref_raw_data = null;

            for (int i = 0; i < ref_names.Length; i++)
            {
                Get_Raw_Data(i); //Get raw data for ref img at index i
                ref_barcodes[i] = QueryImage.Generate_Barcode(ref_raw_data[i], (int)ref_widths[i], (int)ref_heights[i]); //generate barcode for said image
            }
        }

        private async void Get_Raw_Data(int index)
        {
            Windows.Storage.StorageFile file = await StorageFile.GetFileFromPathAsync(ref_directories[index]);

            if (file != null)
            {
                WriteableBitmap bitmap = new WriteableBitmap(64, 64); //create WriteableBitmap with correct pix sizes

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read)) //get byte array from WriteableBitmap
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    bitmap.SetSource(stream);

                    //Scale image
                    BitmapTransform transform = new BitmapTransform()
                    {
                        ScaledWidth = ref_widths[index],
                        ScaledHeight = ref_heights[index]
                    };

                    PixelDataProvider pixeldata = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage
                    );

                    ref_raw_data.Append(pixeldata.DetachPixelData()); //get raw img data
                }
            }
        }
    }
}
