//  <copyright file="ScapeSession.cs" company="Scape Technologies Limited">
//
//  ScapeSession.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    
    /// <summary>
    /// A class to handle the scape session request and response
    /// </summary>
    public abstract class ScapeSession
    {
        /// <summary>
        /// the static instance
        /// </summary>
        private static ScapeSession instance = null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScapeSession" /> class
        /// </summary>
        internal ScapeSession()
        {
            if (instance != null) 
            {
                Debug.Log("There should only be one instance of ScapeSession!");
            }

            instance = this;
        }
        
        /// <summary>
        /// An event that is triggered when a SCape Measurement has been requested
        /// </summary>
        public event Action<double> ScapeMeasurementsRequested;
         
        /// <summary>
        /// An event that is returned when an error occurs getting a ScapeMeasurement
        /// </summary>
        public event Action<ScapeSessionError> ScapeSessionErrorEvent;
        
        /// <summary>
        /// An event that is triggered when the devices location are taken
        /// </summary>
        public event Action<LocationMeasurements> DeviceLocationMeasurementsEvent;
        
        /// <summary>
        /// An event that is triggered when the devices motion are taken
        /// </summary>
        public event Action<MotionMeasurements> DeviceMotionMeasurementsEvent;
        
        /// <summary>
        /// An event that is triggered when the camera transform changes
        /// </summary>
        public event Action<List<double>> CameraTransformEvent;
        
        /// <summary>
        /// An event that is triggered when a ScapeMeasurement has been returned
        /// </summary>
        public event Action<ScapeMeasurements> ScapeMeasurementsEvent;

        /// <summary>
        /// Gets the static ScapeSession instance
        /// Should only be used after the ScapeClient has been started.
        /// </summary>
        public static ScapeSession Instance 
        {
            get 
            {
                return instance;
            }

            private set 
            {
                instance = value;
            }
        }

        /// <summary>
        /// The public function to request a ScapeMeasurement using the given image details
        /// </summary>
        /// <param name="image">
        /// the image to be sent to the Scape back end
        /// </param>
        public abstract void GetMeasurements(ScapeSession.ARImage image);

        /// <summary>
        /// The public function to request a ScapeMeasurement.
        /// The underlying implementation should acquire the image.
        /// </summary>
        public abstract void GetMeasurements();

        /// <summary>
        /// Destroy the ScapeSession object
        /// </summary>
        internal void Terminate()
        {
            instance = null;
        }

        /// <summary>
        /// An internal function to trigger the ScapeMeasurementsRequested from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The timestamp at which the measurement was taken 
        /// </param>
        internal virtual void OnScapeMeasurementsRequested(double arg)
        {
            if (this.ScapeMeasurementsRequested != null) 
            {
                this.ScapeMeasurementsRequested(arg);
            }
        }

        /// <summary>
        /// An internal function to trigger the ScapeSessionErrorEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The ScapeSessionError returned from the ScapeSession 
        /// </param>
        internal virtual void OnScapeSessionErrorEvent(ScapeSessionError arg)
        {
            if (this.ScapeSessionErrorEvent != null) 
            {
                this.ScapeSessionErrorEvent(arg);
            }
        }

        /// <summary>
        /// An internal function to trigger the DeviceLocationMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The LocationMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnDeviceLocationMeasurementsEvent(LocationMeasurements arg)
        {
            if (this.DeviceLocationMeasurementsEvent != null) 
            {
                this.DeviceLocationMeasurementsEvent(arg);
            }
        }

        /// <summary>
        /// An internal function to trigger the DeviceMotionMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The MotionMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnDeviceMotionMeasurementsEvent(MotionMeasurements arg)
        {
            if (this.DeviceMotionMeasurementsEvent != null) 
            {
                this.DeviceMotionMeasurementsEvent(arg);
            }
        }

        /// <summary>
        /// An internal function to trigger the CameraTransformEvent from the underlying implementation
        /// </summary>
        /// <param name="arg">
        /// The camera transform returned from the ScapeSession 
        /// </param>
        internal virtual void OnCameraTransformEvent(List<double> arg)
        {
            if (this.CameraTransformEvent != null) 
            {
                this.CameraTransformEvent(arg);
            }
        }
        
        /// <summary>
        /// An internal function to trigger the ScapeMeasurementsEvent from the underlying implementation
        /// </summary>
        /// <param name="sm">
        /// The ScapeMeasurements returned from the ScapeSession 
        /// </param>
        internal virtual void OnScapeMeasurementsEvent(ScapeMeasurements sm) 
        {
            if (this.ScapeMeasurementsEvent != null) 
            {
                this.ScapeMeasurementsEvent(sm);
            }
        }

        /// <summary>
        /// A struct to hold all information pertaining to an image to be sent to the backend
        /// </summary>
        public struct ARImage 
        {
            /// <summary>
            /// the image Width
            /// </summary>
            public int Width;

            /// <summary>
            /// the image Height
            /// </summary>
            public int Height;

            /// <summary>
            /// the image YPixelBuffer, a pointer to a single byte array representation of the grey scale image.
            /// The buffer should be exactly Width*Height in size bytes.
            /// </summary>
            public IntPtr YPixelBuffer;

            /// <summary>
            /// the image XFocalLength
            /// </summary>
            public float XFocalLength;

            /// <summary>
            /// the image YFocalLength
            /// </summary>
            public float YFocalLength;

            /// <summary>
            /// the image XPrincipalPoint
            /// </summary>
            public float XPrincipalPoint;

            /// <summary>
            /// the image YPrincipalPoint
            /// </summary>
            public float YPrincipalPoint;

            /// <summary>
            /// the image IsAvailable
            /// </summary>
            public bool IsAvailable;
        }
    }
}
