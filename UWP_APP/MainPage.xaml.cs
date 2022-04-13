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
using Windows.Storage.AccessCache;
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
        private List<string> ref_directories = new List<string>();
        private List<string> ref_names = new List<string>();
        private List<List<byte>> ref_raw_data =  new List<List<byte>>(); //[ref_item][byte]
        private List<uint> ref_widths = new List<uint>();
        private List<uint> ref_heights = new List<uint>();
        List<List<int>> ref_barcodes = new List<List<int>>();

        List<float> distances = new List<float>();

        private string query_directory = "";
        private string query_name = "";
        private List<byte> query_raw_data = new List<byte>();
        private int query_width;
        private int query_height;
        List<int> query_barcode;

        private int input_select_index = -1;
        private List<string> FAL_tokens = new List<string>(); //tokens for future access list

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

            //Clear FAL data
            FALManip.ClearFAL();
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

        private void NavigationViewControl_ItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
        {
            object navitem_name = args.InvokedItem;
            input_select_index = ref_names.IndexOf(navitem_name.ToString());
            Set_Ref_Image(input_select_index);
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
                query_name = file.Name; //save file name
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

                    query_raw_data = pixeldata.DetachPixelData().ToList(); //get raw img data
                }

                img_query.Source = bitmap; //Update query image display on ui
                tb_queryimage.Text = "Query Image - " + query_name; //Update image name display on ui

                if (ref_names.Count > 0)
                {
                    CompareButton.IsEnabled = true;
                }
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
                    ref_directories.Add(file.Path); //add file directories to array
                    ref_names.Add(file.Name); //add file names to array
                    uint image_width = properties.Width;
                    uint image_height = properties.Height;
                    ref_widths.Add(image_width);
                    ref_heights.Add(image_height);

                    MUXC.NavigationViewItem navitem = new MUXC.NavigationViewItem(); //Add new item to NavigationView list to display file name
                    navitem.Content = file.Name;
                    navitem.Icon = new SymbolIcon(Symbol.Target);
                    NavigationViewControl.MenuItems.Add(navitem);


                    FAL_tokens.Add(FALManip.RememberFile(file)); //Save file ref in FAL
                }
                if (query_raw_data.Count > 0)
                {
                    CompareButton.IsEnabled = true;
                }
            }
        }

        /*
        private async void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                ref_directories.Add(folder.Path);
                ref_names.Add(folder.Name);

                //TODO: Update list on side to include new folder

                
                //Windows.Storage.AccessCache.StorageApplicationPermissions.
                //FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                
            }
        }
        */

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (input_select_index != -1)
            {
                NavigationViewControl.MenuItems.RemoveAt(input_select_index + 1);
                ref_names.RemoveAt(input_select_index);
                ref_directories.RemoveAt(input_select_index);
                ref_widths.RemoveAt(input_select_index);
                ref_heights.RemoveAt(input_select_index);
                FALManip.RemoveFileByToken(FAL_tokens[input_select_index]);
            }
            if (ref_names.Count == 0)
            {
                input_select_index = -1;
                CompareButton.IsEnabled = false;
            }
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            query_barcode = QueryImage.Generate_Barcode(query_raw_data, query_width, query_height);

            //reset past results
            ref_barcodes.Clear();
            ref_raw_data.Clear();
            distances.Clear();

            for (int i = 0; i < ref_names.Count; i++)
            {
                Get_Raw_Data(i); //Get raw data for ref img at index i
                ref_barcodes.Add(QueryImage.Generate_Barcode(ref_raw_data[i], (int)ref_widths[i], (int)ref_heights[i])); //generate barcode for said image
                distances.Add(QueryImage.HammingDistance("", "")); //compare query barcode with ref barcode
            }

            //TODO: Connect ref_barcodes to HammingDistance
            //TODO: Add output
            //output
        }

        private async void Get_Raw_Data(int index)
        {
            Windows.Storage.StorageFile file = await FALManip.GetFileForToken(FAL_tokens[index]);

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

                    ref_raw_data.Add(pixeldata.DetachPixelData().ToList()); //get raw img data
                }
            }
        }

        private async void Set_Ref_Image(int index)
        {
            Windows.Storage.StorageFile file = await FALManip.GetFileForToken(FAL_tokens[index]);

            if (file != null)
            {
                WriteableBitmap bitmap = new WriteableBitmap(64, 64); //create WriteableBitmap with correct pix sizes

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read)) //get byte array from WriteableBitmap
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    bitmap.SetSource(stream);
                }

                img_ref.Source = bitmap; //Update ref image display on ui
                tb_referenceimage.Text = "Reference Image - " + ref_names[index]; //Update image name display on ui
            }
        }

    }
}
