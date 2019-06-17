//  <copyright file="UnityScapeARGeneratePlane.cs" company="Scape Technologies Limited">
//
//  UnityScapeARGeneratePlane.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR.iOS;

    /// <summary>
    /// UnityScapeARGeneratePlane, makes use of Scape's UnityScapeARAnchorManager
    /// </summary>
    public class UnityScapeARGeneratePlane : MonoBehaviour
    {
        /// <summary>
        /// The game prefab used to render the planes
        /// </summary>
        [SerializeField]
        private GameObject planePrefab;

        /// <summary>
        /// unityARAnchorManager the component that tracks the plane's 
        /// detected by ARKit
        /// </summary>
        private UnityScapeARAnchorManager unityARAnchorManager;

        /// <summary>
        /// Start function
        /// </summary>
        public void Start() 
        {
            unityARAnchorManager = new UnityScapeARAnchorManager();
            UnityARUtility.InitializePlanePrefab(planePrefab);
        }

        /// <summary>
        /// OnDestroy function
        /// </summary>
        public void OnDestroy()
        {
            unityARAnchorManager.Destroy();
        }
    }
}