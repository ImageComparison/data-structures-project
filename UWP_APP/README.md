# Source Code Directory
- [Repository Directory](../README.md)

## UWP Application
[Project Solution](UWP_APP.sln)
- [App.xaml](App.xaml)
- [App.xaml.cs](App.xaml.cs)

## Main Window Code
#### [MainPage.xaml](MainPage.xaml)
This includes the markup for the GUI design. It uses a couple NuGet Packages to emulate the WinUI 3 aesthetic.
#### [MainPage.xaml.cs](MainPage.xaml.cs)
This is the C# code which implements the design of the GUI into the code while also providing function to the GUI (Buttons, Navigation, etc.).
EventHandlers trigger various sections of code based on user inputs.

## External Classes
#### [QueryImage.cs](QueryImage.cs)
This class contains all methods for generating barcodes, projections, and hamming distances. These methods are called by the MainPage for each image.
#### [FALManip.cs](FALManip.cs)
This class is solely made to provide an interface for Windows' Future Access List (FAL) API, which allows the app to access previously opened files without requesting the user for permission each time. This is used to access images when displayed and image raw data when generating barcodes for reference images.
