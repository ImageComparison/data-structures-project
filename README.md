# Data Structures Project (Group 24)
Repository for our SOFE 2715 group project

- [Project Report Document](https://docs.google.com/document/d/1zSLPa5YUdwerXrFHG5TFrGs3dYmOwx3sfJro5BFAWiw/edit?usp=sharing)
- [Project Report Slideshow](https://docs.google.com/presentation/d/1tw6zD4_-0BbDfXxqoV31a8dApKX4AWhku_c9c1enzqk/edit?usp=sharing)
- [Most Current Source Code](/App/UWP)
- [Source Code Directory](/App/UWP/README.md)

# Repository Directory
## Images  
Holds the sample images given by the professor as well as some images Alden has used for debugging.

[/images](/images)
- [/profs-images](/images/profs-images)
- [/single-images](/images/single-images)

## Notes
Txt files used during the construction of the project for note taking.

[/notes](/notes)

## Python Scripts
Version of the project written in Python 3. This is the part of the project Alden worked on. 
Contains most of the projection mapping and hamming distance code for this project.

[/pythonScripts](/pythonScripts)
- [/hammingDistance](/pythonScripts/hammingDistance)


## Query Images
Folder used for getting query images in python script.

[/queryIMGs](/queryIMGs)

## App
Initially John Howe ([@johnh-otu](https://github.com/orgs/ImageComparison/people/johnh-otu)) made a UI prototype for `UWP` (using WinUI 2). This directory was quickly abandoned for `UWP_APP` to separate the prototype from the current version. The `UWP_APP` folder was later renamed to `/App/UWP`.

The most up-to-date version of the final project.
[Source Code Directory](/App/UWP/)

<!-- > To build the project I highly suggest following this tutorial to install all the tools required. [docs.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=vs-2022-17-1-a%2Cvs-2022-17-1-b) -->

[/App](/App)
- [/Publish](/App/Publish) - Installable files are located here, click the 
  - [/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test](/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test) - Verification Certificate and MSIX files are here. 
  - > Make sure to install the [certificate](/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test/ImageComparisonMSIX_1.0.2.0_x64_Debug.cer) first, then install the [MSIX package](/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test/ImageComparisonMSIX_1.0.2.0_x64_Debug.msix).
- [/ImageComparisonMSIX](/App/ImageComparisonMSIX) - MSIX Packager, it packages and encrypts the App using a certificate
- [/UWP](/App/UWP) - Source files for the UWP app
- [/UWP_APP.sln](/App/UWP_APP.sln) - Visual Studio Solution file (click this file to open the project)

### Install app
To install the final project all you need to do is to go to [/App/Publish/](/App/Publish/) and download [/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test](/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test) onto your computer, from thhere go into the downloaded `/App/Publish/` folder, then install the public key certificate for the app, `/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test/ImageComparisonMSIX_1.0.2.0_x64_Debug.cer` (double click the app) after which install the MSIX package `/App/Publish/ImageComparisonMSIX_1.0.2.0_x64_Debug_Test/ImageComparisonMSIX_1.0.2.0_x64_Debug.msix` (double click the msix app).

Step 1:

![image](https://user-images.githubusercontent.com/91390448/163497105-8633a242-1226-47a6-887b-15db0bc45d6f.png)

Step 2:

![image](https://user-images.githubusercontent.com/91390448/163497126-6f0da7b9-3fb1-4764-bfc3-96f66dc1b553.png)

Step 3:

![image](https://user-images.githubusercontent.com/91390448/163497164-6261f516-9cf6-4ac6-ae6c-422cd5ca5be2.png)

Step 4:

![image](https://user-images.githubusercontent.com/91390448/163497188-25a33fcb-a702-438e-a1e4-79c38e2ea3d2.png)

Step 5:

![image](https://user-images.githubusercontent.com/91390448/163497241-9c9c8b82-f676-45d9-a478-4449bb36e6ed.png)

Step 6:

![image](https://user-images.githubusercontent.com/91390448/163497260-3af41ad4-716d-45fc-b2eb-e9f56880fd43.png)

Step 7:

![image](https://user-images.githubusercontent.com/91390448/163497335-4ee87aa1-aa6e-41bb-9307-ab120d05d4e6.png)

Step 8:

![image](https://user-images.githubusercontent.com/91390448/163497409-024dc9f2-6f3d-4d56-a454-43d0a69cd891.png)

Step 9:

![image](https://user-images.githubusercontent.com/91390448/163497468-381b8f28-9e3d-46a1-b96d-e451d537fc44.png)

Step 10:

![image](https://user-images.githubusercontent.com/91390448/163497510-094f34b0-70ca-4fd5-beaf-927842702936.png)

Step 11 (Final App Launch 🎉, bon-apettite...):

![image](https://user-images.githubusercontent.com/91390448/163497734-a5f40b0e-58d4-47be-b82b-18681e1794a6.png)

### Building app

> To build the project I highly suggest following this tutorial to install all the tools required. [docs.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=vs-2022-17-1-a%2Cvs-2022-17-1-b)

Open Visual Studio, go to the `/App/` folder, then click `/UWP_APP.sln` the project should be setup ready for you to go.

## WinUI
Version of the project written in .NET framework for WinUI 3. This is the prototype John worked on before the project switched to UWP.

[/winui](/winui)
