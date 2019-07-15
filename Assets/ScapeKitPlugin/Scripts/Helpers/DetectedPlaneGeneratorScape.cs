//-----------------------------------------------------------------------
// <copyright file="DetectedPlaneGeneratorScape.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Modifications copyright (C) 2019 company="Scape Technologies"
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.Common
{
    using System;
    using System.Collections.Generic;
    using GoogleARCore;
    using ScapeKitUnity;
    using UnityEngine;

    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// </summary>
    public class DetectedPlaneGeneratorScape : MonoBehaviour, IGeoOrigin
    {
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        [SerializeField]
        private GameObject detectedPlanePrefab;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is
        /// used across the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> newPlanes = new List<DetectedPlane>();

        /// <summary>
        /// a boolean to hold whether the scape measurement has come in yet
        /// </summary>
        private bool hadOriginEvent = false;

        /// <summary>
        /// register self with GeoAnchorManager so made aware when measurement is taken 
        /// </summary>
        public void Start()
        {
            GeoAnchorManager.Instance.RegisterGeoInterface(this);
        }

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Session.GetTrackables<DetectedPlane>(newPlanes, TrackableQueryFilter.New);
            for (int i = 0; i < newPlanes.Count; i++)
            {
                ScapeLogging.LogDebug(message: "new plane!");
                Transform parent = GeoCameraComponent.WorldTransform;

                GameObject planeObject =
                    Instantiate(detectedPlanePrefab, Vector3.zero, Quaternion.identity, parent);

                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(newPlanes[i]);
            }
        }

        /// <summary>
        /// After a scape measurement arrives parent all the plane game objects
        /// to the world transform.
        /// </summary>
        public void OriginEvent() 
        {
            hadOriginEvent = true;
        }
    }
}
