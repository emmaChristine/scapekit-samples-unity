# ScapeKit Samples Unity (using ScapeKit 1.0.3)

Sample scenes demonstrating usage of the SceneKit SDK in Unity

This repo contains the Scapekit SDK, GoogleARCore and ARKit packages for Unity.

Currently samples are platform specific. Sample scenes are provided for Android and iOS separately.

To begin check out this repo and open it at it's root folder into Unity.

These samples were built using Unity 2018.3.8f1.


## Acquire a Scapekit Developer key

https://developer.scape.io/documentation/ 

Once Unity has started find the ScapeKit menu, and select "Account"
ScapeKit's build helper will appear in a panel in Unity. Here you will need to add your Scapekit developer's API Key.

## Geo Samples

Example scenes implementing geo located content are available for both ARKit and ARCore. Again these are platform specific:

1. Assets/Scenes/ScapekitGeoSceneAndroid.unity
2. Assets/Scenes/ScapekitGeoSceneIOS.unity

### ScapeCamera Prefabs

The ScapeCamera(ARCore|ARKit) prefabs still relies on the AR platforms for immediate control of the camera.
However with the replacement of the ScapeCamera the AR camera system is transformed into the context of a real world geolocation and heading.

This component responds to a successful Scape Vision Engine result by updating geo anchored objects positions in the scene relative to the camera and adjusting the camera's vertical orientation to make it match that of a compass.  

### GeoAnchors

In order to make 3D content geo located a "GeoAnchor" component should be added to a game object. The GeoAnchor component takes longitude and latitude variables.  
When the ScapeCameraCompoent receives a successful Vision Engine result, all game objects parented to a GeoAnchor will be moved to the geo correct location within the scene, relative to the camera.

### Data Driven Samples

1. Assets/Scenes/GeoDataAndroid
2. Assets/Scenes/GeoDataIOS

These samples show a simple method for using a data file to populate a scene with objects given real world coordinates.

The source code can be found:

"Assets/GeoAssets/OpenDataFile.cs"

The data file can be found:

"Assets/Resources/streetdata.json"

Note that the prefabs that are loaded from the file are also located in the resources folder.

