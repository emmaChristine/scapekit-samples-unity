//  <copyright file="GeoCameraComponent.cs" company="Scape Technologies Limited">
//
//  GeoCameraComponent.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.XR;

    /// <summary>
    /// GeoCameraComponent. Can be added to any camera component to apply the Scape functionality too. 
    /// When a ScapeMeasurement is returned to the ScapeSessionManager that updates the GeoCmeraComponent.
    /// The GeoCameraComponent inserts a new GameObject above the camera it is attached to.
    /// The GameObject's transform takes the camera into a globally correct world space.
    /// In that world space the z axis is always true north and the x axis east. The world position of the camera
    /// is relative to the center of the S2Cell (and the scene's origin) found in the GeoAnchorManager.
    /// </summary>
    public class GeoCameraComponent : MonoBehaviour
    {
        /// <summary>
        /// The main camera to apply the scape global transform too
        /// </summary>
        [SerializeField]
        private Camera theCamera;

        /// <summary>
        /// The worldTransformObject is used to convert the local camera's space (typically set by ARKit/Core)
        /// to the global reference space. That is a space relative to the S2 cell being used as the point of origin for the scene
        /// and the direction being relative to north.
        /// The world transform object is made the camera's parent transform.
        /// </summary>
        private GameObject worldTransformObject = null;

        /// <summary>
        /// The camera position at which point scape measurements last were taken
        /// </summary>
        private Vector3 positionAtScapeMeasurements;

        /// <summary>
        /// The camera rotation at which point scape measurements last were taken
        /// </summary>
        private Vector3 rotationAtScapeMeasurements;

        /// <summary>
        /// The camera's position relative to the root s2cell (it's intended world position, as established by VPS)
        /// calculated at the point a scape measurement is returned, mainly for debugging purposes
        /// </summary>
        private Vector3 cameraS2Position;

        /// <summary>
        /// worldTransformDirection. The rotation component of the worldTransform object.
        /// </summary>
        private float worldTransformDirection = 0.0f;

        /// <summary>
        /// worldTransformPosition, The position component of the worldTransform object
        /// </summary>
        private Vector3 worldTransformPosition = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// used to update worldTransform in Unity main thread
        /// </summary>
        private bool updateRoot = false;

        /// <summary>
        /// used to save the camera's position and orientation at the point the Scape Measurements are taken
        /// </summary>
        public void HoldCameraPose()
        {
            positionAtScapeMeasurements = theCamera.transform.localPosition;
            rotationAtScapeMeasurements = theCamera.transform.localRotation.eulerAngles;
        }

        /// <summary>
        /// create camera parent at start
        /// </summary>
        public void Start() 
        {
            SetupCameraParent();
        }

        /// <summary>
        /// update root on main thread
        /// </summary>
        public void Update()
        {
            UpdateRoot();
        }

        /// <summary>
        /// called by ScapeSessionComponent. 
        /// Here the scape measurement is used to update the world transform object 
        /// in order to position and orient the camera with respect to the scene's origin.
        /// </summary>
        /// <param name="coordinates">
        /// GPS Coordinates given by scape measurements
        /// </param>
        /// <param name="heading">
        /// The compass heading given by scape measurements
        /// </param>        
        public void SynchronizeARCamera(LatLng coordinates, float heading) 
        {
            ScapeLogging.LogDebug(message: "SynchronizeARCamera() LatLngCoordinates = " + ScapeUtils.CoordinatesToString(coordinates));

            ScapeLogging.LogDebug(message: "SynchronizeARCamera() ARHeading = " + rotationAtScapeMeasurements.y);
            ScapeLogging.LogDebug(message: "SynchronizeARCamera() ARPosition = " + positionAtScapeMeasurements.ToString());
         
            // the Unity position the camera should be in, that is it's position relative to the S2 cell based on it's
            // gps coordinates
            cameraS2Position = ScapeUtils.WgsToLocal(
                                                    coordinates.Latitude, 
                                                    coordinates.Longitude, 
                                                    0.0, 
                                                    GeoAnchorManager.Instance.S2CellId);
            
            // the world transform direction corrects the camera's Heading to be relative to North.
            worldTransformDirection = heading - rotationAtScapeMeasurements.y;

            if (worldTransformDirection < 0.0) 
            {
                worldTransformDirection += 360.0f;
            }

            ScapeLogging.LogDebug(message: "SynchronizeARCamera() worldTransformDirectionYAngle = " + worldTransformDirection);
            
            Vector3 positionAtScapeMeasurementsRotated = Quaternion.AngleAxis(worldTransformDirection, Vector3.up) * positionAtScapeMeasurements;

            // the world transform position corrects the camera's final position after applying the direction correction
            worldTransformPosition = cameraS2Position - positionAtScapeMeasurementsRotated;
            ScapeLogging.LogDebug(message: "SynchronizeARCamera() worldTransformPosition = " + worldTransformPosition.ToString());

            updateRoot = true;
        }

        /// <summary>
        /// create the world tranform object and insert it above the camera object in the scene
        /// </summary>
        private void SetupCameraParent()
        {
            if (!theCamera) 
            {
                theCamera = Camera.main;
            }

            var cameraParent = theCamera.transform.parent;

            worldTransformObject = new GameObject();
            theCamera.transform.SetParent(worldTransformObject.transform, false);

            if (cameraParent) 
            {
                worldTransformObject.transform.SetParent(cameraParent.transform, false);
            }
        }

        /// <summary>
        /// update the world transform object having been given new scape measurements
        /// </summary>
        private void UpdateRoot() 
        {
            if (updateRoot) 
            {
                PrintError();

                ScapeLogging.LogDebug(message: "GeoCameraComponent::UpdateRoot()");

                worldTransformObject.transform.rotation = Quaternion.AngleAxis(worldTransformDirection, Vector3.up);
                worldTransformObject.transform.position = worldTransformPosition;
                
                updateRoot = false;
            }
        }

        /// <summary>
        /// print some debug output for logging purposes
        /// </summary>
        private void PrintError() 
        {
            ScapeLogging.LogDebug(message: "CameraS2Position = " + cameraS2Position.ToString());
            ScapeLogging.LogDebug(message: "CameraCWPosition = " + theCamera.transform.position.ToString());
        }
    }
}