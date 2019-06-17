//  <copyright file="GeoAnchorManager.cs" company="Scape Technologies Limited">
//
//  GeoAnchorManager.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using UnityEngine;

    /// <summary>
    /// There must be one and only one instance of this class in a scene 
    /// that uses Scapekit's GeoAnchor components.
    /// This class holds the id of an S2Cell, the center of which defines the World Coordinate 
    /// for the origin of the Unity scene. 
    /// This class activates the GeoAnchored components the first time a successful Scape Measurement is
    /// received by the ScapeSessionManager.
    /// This is implemented as a [MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html),
    /// in order to be used in Unity. The [GameObject](https://docs.unity3d.com/ScriptReference/GameObject.html) is
    /// irrelevant to it's functionality.
    /// </summary>
    public class GeoAnchorManager : MonoBehaviour
    {
        /// <summary>
        /// The GeoOriginEvent action is used to broadcast the ScapeMeasurements updates to the GeoAnchor components
        /// </summary>
        private static Action geoOriginEvent;

        /// <summary>
        /// The Coordinates of the S2Cell being used as the origin of the Unity scene.
        /// </summary>
        private static LatLng s2CellCoordinates;

        /// <summary>
        /// The single instance of the geoAnchorManager for the scene.
        /// </summary>
        private static GeoAnchorManager geoAnchorManager = null;

        /// <summary>
        /// The GPS Longitude in degrees used to define the root of the Unity Scene.
        /// If the default value (-1.0), is used the root is decided at the first Scape Measurement
        /// </summary>
        [SerializeField]
        private double longitude = -1.0;

        /// <summary>
        /// The GPS Latitude in degrees used to define the root of the Unity Scene.
        /// If the default value (-1.0), is used the root is decided at the first Scape Measurement
        /// </summary>
        [SerializeField]
        private double latitude = -1.0;

        /// <summary>
        /// The S2Cell id for the GeoAnchorManager.
        /// </summary>
        private long s2CellId = 0;

        /// <summary>
        /// isInstantiated. Becomes true after the first Scape Measurement comes in.
        /// </summary>
        private bool isInstantiated = false;

        /// <summary>
        /// Used to do the Origin event updates on the main thread
        /// </summary>
        private bool doOriginEvent = false;

        /// <summary>
        /// Gets or sets the LatLng of the origin.
        /// These are the LatLng of the S2Cell's center.
        /// After a scape measurement has come in the S2CellLatLng for this GeoAnchorManager
        /// are treated as the world LatLng of the origin of the Unity scene.
        /// </summary>
        public static LatLng S2CellCoordinates
        {
            get 
            { 
                return s2CellCoordinates; 
            }

            protected set 
            { 
                s2CellCoordinates = value; 
            }
        }

        /// <summary>
        /// Gets the static instance of the geoAnchorManager for the scene.
        /// </summary>
        public static GeoAnchorManager Instance
        {
            get 
            {
                if (geoAnchorManager == null)
                {
                    ScapeLogging.LogError(message: "Error: No GeoAnchorManager component detected in scene!");
                }

                return geoAnchorManager;
            }
        }

        /// <summary>
        /// Gets or sets The S2Cell id.
        /// </summary>
        public long S2CellId 
        {
            get 
            { 
                if (s2CellId == 0) 
                {
                    ScapeLogging.LogError(message: "Error: S2CellId has not yet been decided.\n" +
                        "Longitude and Latitude coordinates should be set on the GeoRootWorld object in Unity Editor.\n" +
                        "Otherwise the app must wait for cooordinates from Scape Measuremnts or GPS to be received\n" +
                        "before making use of the S2CellId.");
                }

                return s2CellId;
            }

            protected set 
            { 
                s2CellId = value; 
            }
        }

        /// <summary>
        /// When the object awakes, call the StaticInit.
        /// There shold only be one such object and thus this call should only be called once in any Unity scene.
        /// </summary>
        public void Awake() 
        {
            StaticInit();
        }

        /// <summary>
        /// After a ScapeMeasurement has come in, update all GeoAnchors by calling the GeoOriginEvent action
        /// This must be done on Unity's main thread.
        /// </summary>
        public void Update() 
        {
            if (doOriginEvent) 
            {
                ScapeLogging.LogDebug(message: "GeoAnchorManager::geoOriginEvent()");
                geoOriginEvent();
                doOriginEvent = false;
            }
        }

        /// <summary>
        /// InstantiateOrigin is called by the main camera when a successful scape meaasurements event happens.
        /// The Coordinates passed in are the Geo Location of Unity's origin.
        /// </summary>
        /// <param name="rootSessionCoords">
        /// The GPS coordinates for the oriing of the Unity scene,
        /// </param>
        public void InstantiateOrigin(LatLng rootSessionCoords) 
        {
            ScapeLogging.LogDebug(message: "GeoAnchorManager::InstantiateOrigin()");
            if (longitude == -1.0 || latitude == -1.0) 
            {
                ScapeLogging.LogDebug(message: "Longitude and Latitude coordinates not set, " +
                    "using coordinates returned form Scape Measurements for root s2cell");

                longitude = rootSessionCoords.Longitude;
                latitude = rootSessionCoords.Latitude;

                FindS2CellCoordinates();
            }

            isInstantiated = true;

            if (geoOriginEvent != null) 
            {
                doOriginEvent = true;
            }
        }

        /// <summary>
        /// Each GeoAnchored GameObject registers itself to the GeoAnchorManager singleton.
        /// This sets it's "OriginEvent" function to be called when a successful scape measurement comes in.        
        /// </summary>
        /// <param name="geoOrigin">
        /// The object implementing GeoOriginInterface registering itself with the GeoAnchorManager
        /// </param>
        public void RegisterGeoInterface(IGeoOrigin geoOrigin)
        {
            // append the GeoAnchor's OriginEvent function to the GeoOriginEvent action
            geoOriginEvent += geoOrigin.OriginEvent;

            // if we already have received a scape measurement we immediately call 
            // the OriginEvent.
            if (isInstantiated)
            {
                geoOrigin.OriginEvent();
            }
        }

        /// <summary>
        /// Initializes the GeoAnchorManager, should only happen once
        /// Sets the S2Cell if the user has given GPS coordinates, other wise that is done later
        /// when the ScapeMeasurement comes in.
        /// </summary>
        private void StaticInit() 
        {
            ScapeLogging.LogDebug(message: "GeoAnchorManager::StaticInit()");

            if (geoAnchorManager != null) 
            {
                ScapeLogging.LogError(message: "Error: more than one GeoAnchorManager detected in scene!");
            }
            else 
            {
                geoAnchorManager = this;
            }

            if (longitude != -1.0 && latitude != -1.0) 
            {
                FindS2CellCoordinates();
            }
        }

        /// <summary>
        /// Identify which S2Cell will be used for the root. 
        /// Find the S2Cell's GPS Coordinates.
        /// </summary>
        private void FindS2CellCoordinates() 
        {
            S2CellId = ScapeUtils.CellIdForWgs(latitude, longitude);

            S2CellCoordinates = ScapeUtils.LocalToWgs(new Vector3(0, 0, 0), S2CellId);

            ScapeLogging.LogDebug(message: "GeoAnchorManager::S2CellId = " + S2CellId.ToString("X"));
            ScapeLogging.LogDebug(message: "GeoAnchorManager::S2CellCoordinates = " + ScapeUtils.CoordinatesToString(S2CellCoordinates));
        }
    }
}