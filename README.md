# ScapeKit Samples Unity

Sample scenes demonstrating usage of the SceneKit SDK in Unity

This repo contains the Scapekit SDK, GoogleARCore and ARKit packages for Unity.

Currently samples are platform specific. Sample scenes are provided for Android and iOS separately.

"Build And Run" does not currently work with ScapeKit. Details on deplying sample apps given below.

To begin check out this repo and open it at it's root folder into Unity.

These samples were built using Unity 2018.2.1f1.

## Aquire a Scapekit Developer key

https://developer.scape.io/documentation/ 

Once Unity has started find the ScapeKit menu, and select "Build And Deploy"
ScapeKit's build helper will appear in a panel in Unity. Here you will need to add your Scapekit developer's API Key.

Here there are options to build and deploy the apps at the press of a button. Howerver this is a work in progress so below are details of the steps to manually export the sample scenes for both Android. and iOS.

## Building Simple Scene for Android

You should already have the latest version of Android Studio installed on your machine.

Open the scene Assets/Scenes/ScapekitSimpleSceneAndroid

Open File -> Build Settings. Select Android.
Open Build Settings -> Player Settings

In the Player settings tab enter company name, product name.
Under "Other Settings" enter Package Name "com.COMPANYNAME.PRODUCTNAME"
For Target Architecture make sure only "ARM64" is selected (Scapekit is only built for this architecture currently).
Under "XR Settings" make sure "ARCore Supported" is checked.

Back in Build Settings on the Android tab check "Export project".
Then hit the "Export" button.
Unity will show a file dialog for you to choose the folder to export your Android project too. Once this is finished, open the project in Android Studio to complete the deployment process to device.

### Android Studio

The first time you open the sample app in Android Studio, if you are using the latest (3.3.1), you maybe asked some inital questions regarding the inttegration of hte project. Our recommendations are:

Gradle Sync: "Would you like to recreate the gradle wrapper..."

 - Put OK for this.

 Android Gradle Plugin Upgrade Recommended

 - Don't remind me again for this project.

### Permissions!

The current examples do not include a method of requesting permissins from the user. After installing the sample for the first time, go into the device's settings, find the sample app and enable permissions for Camera and Location.

A simple implementation to request permissions from the user in Unity can be found here:

https://github.com/Over17/UnityAndroidPermissions

## Building Simple Scene for iOS

You should already have the latest version of XCode installed on your machine along with a develeper license.

Open the scene Assets/Scenes/ScapekitSimpleSceneiOS

Open File -> Build Settings. Select iOS.
Open Build Settings -> Player Settings
In the Player settings tab enter company name, product name.

Under "Other Settings" enter a bundle identifier.
Under "Camera Usage Description" and "lLcation Usage Description" enter something (exaclty what will only become crucial once you export to the App Store), but any value installs permissions for the app.

Back in "Build Settings" hit "Build". Unity will show a file dialog for you to choose the folder to export your XCode project too. Once this is finished, open the project in XCode to complete the deployment process to device.

## Geo Samples

Example scenes implementing geo located comtent are available for both ARKit and ARCore. Again these are platform specific:

1. Assets/Scenes/ScapekitGeoSceneAndroid.unity
2. Assets/Scenes/ScapekitGeoSceneIOS.unity

### ScapeCameraCompoent

The ScapeCameraComponent(ARCore|ARKit) still relies on the AR platfoms for immediate control of the camera.
However with the replacement of the ScapeCameraComponent the AR camera system is placed into the context of real world geolocation and heading.

The ScapeCameraComponent(s) replace key components of ARCore/Kit. 

On ARCore the "TrackedPoseDriver" component is replaced, this is ususally applied to the main camera object.
On ARKit the ScapeCamera replaces the ARCameraManager, again this compoenent can be applied to the main camera.

This component responds to a successful Scape Vision Engine result by updating geo anchored objects positions in the scene relative to the camera and adjusting the camera's vertical orientation to make it match that of a compass.  

### GeoAnchors

In order to make 3D content geo located a "GeoAnchor" component should be added to a game object. The GeoAnchor component takes longitude and latitude variables.  
When the ScapeCameraCompoent receives a successful Vision Engine result, all game objects parented to a GeoAnchor will be moved to the geo correct location within the scene, relative to the camera.
