# ScapeKit Samples Unity

Sample scenes demonstrating usage of the SceneKit SDK in Unity

This repo contains the Scapekit SDK, GoogleARCore and ARKit packages for Unity.

Currently samples are platform specific. Sample scenes are provided for Android and iOS separately.

To begin check out this repo and open it at it's root folder into Unity.

These samples were built using Unity 2018.3.8f1.


## Aquire a Scapekit Developer key

https://developer.scape.io/documentation/ 

Once Unity has started find the ScapeKit menu, and select "Build And Deploy"
ScapeKit's build helper will appear in a panel in Unity. Here you will need to add your Scapekit developer's API Key.

Here there are options to build and deploy the apps at the press of a button. Howerver this is a work in progress so below are details of the steps to 

### Permissions!

The current examples do not include a method of requesting permissins from the user. After installing the sample for the first time, go into the device's settings, find the sample app and enable permissions for Camera and Location.

A simple implementation to request permissions from the user in Unity can be found here:

https://github.com/Over17/UnityAndroidPermissions

## Simple Scene

The simple scene doesn not use the AR Camera. A button is displayed in the app to get Scape Measurements.
At the moment the button is pressed the app will send the image to teh Scape backend. If the location is recognized a success message is displayed in teh text box on screen.

## Geo Samples

Example scenes implementing geo located content are available for both ARKit and ARCore. Again these are platform specific:

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
