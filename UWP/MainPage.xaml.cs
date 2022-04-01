using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTest_DataStructuresAssignment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string query_directory = "";
        private byte[] query_raw_data;

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void OnClick1(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if(file != null)
            {
                ImageProperties properties = await file.Properties.GetImagePropertiesAsync(); //get image height and width
                uint image_width = properties.Width;
                uint image_height = properties.Height;

                query_directory = file.Path; //save file path

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read)) //get byte array from WriteableBitmap
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                    //Scale image
                    BitmapTransform transform = new BitmapTransform() { 
                        ScaledWidth = image_width, 
                        ScaledHeight = image_height };

                    PixelDataProvider pixeldata = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage
                    );

                    query_raw_data = pixeldata.DetachPixelData();
                    Console.WriteLine();
                }
            }
        }

        public void OnClick2(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
