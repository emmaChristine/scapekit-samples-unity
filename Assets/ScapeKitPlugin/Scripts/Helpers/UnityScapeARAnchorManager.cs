//  <copyright file="UnityScapeARAnchorManager.cs" company="Scape Technologies Limited">
//
//  UnityScapeARAnchorManager.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Collections.Hybrid.Generic;
    using UnityEngine;
    using UnityEngine.XR.iOS;

    /// <summary>
    /// Manages the visualization of detected planes in the scene.
    /// This only  differs from ARKit's original in that the planes are parented 
    /// to the GeoCameraComponent's WorldTransform so they are displayed correctly after
    /// a scape measurement has come in.
    /// </summary>
    public class UnityScapeARAnchorManager 
    {
        /// <summary>
        /// a map holding planes keyed by anchor id
        /// </summary>
        private LinkedListDictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityScapeARAnchorManager" /> class.
        /// </summary>
        public UnityScapeARAnchorManager()
        {
            this.planeAnchorMap = new LinkedListDictionary<string, ARPlaneAnchorGameObject>();
            UnityARSessionNativeInterface.ARAnchorAddedEvent += this.AddAnchor;
            UnityARSessionNativeInterface.ARAnchorUpdatedEvent += this.UpdateAnchor;
            UnityARSessionNativeInterface.ARAnchorRemovedEvent += this.RemoveAnchor;
        }

        /// <summary>
        /// AddAnchor function
        /// </summary>
        /// <param name="arPlaneAnchor">
        /// ARPlaneAnchor plane callback 
        /// </param>
        public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            GameObject go = UnityARUtility.CreatePlaneInScene(arPlaneAnchor);

            ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject();
            arpag.planeAnchor = arPlaneAnchor;
            arpag.gameObject = go;

            ScapeLogging.LogDebug(message: "UnityScapeARAnchorManager::AddAnchor()");

            arpag.gameObject.transform.SetParent(GeoCameraComponent.WorldTransform);

            this.planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);
        }

        /// <summary>
        /// RemoveAnchor function
        /// </summary>
        /// <param name="arPlaneAnchor">
        /// ARPlaneAnchor plane callback 
        /// </param>
        public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            if (this.planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) 
            {
                ARPlaneAnchorGameObject arpag = this.planeAnchorMap[arPlaneAnchor.identifier];
                GameObject.Destroy(arpag.gameObject);
                this.planeAnchorMap.Remove(arPlaneAnchor.identifier);
            }
        }

        /// <summary>
        /// UpdateAnchor function
        /// </summary>
        /// <param name="arPlaneAnchor">
        /// ARPlaneAnchor plane callback 
        /// </param>
        public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            if (this.planeAnchorMap.ContainsKey(arPlaneAnchor.identifier)) 
            {
                ARPlaneAnchorGameObject arpag = this.planeAnchorMap[arPlaneAnchor.identifier];
                UnityARUtility.UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
                arpag.planeAnchor = arPlaneAnchor;
                this.planeAnchorMap[arPlaneAnchor.identifier] = arpag;
            }
        }

        /// <summary>
        /// UnsubscribeEvents function
        /// </summary>
        public void UnsubscribeEvents()
        {
            UnityARSessionNativeInterface.ARAnchorAddedEvent -= this.AddAnchor;
            UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= this.UpdateAnchor;
            UnityARSessionNativeInterface.ARAnchorRemovedEvent -= this.RemoveAnchor;
        }

        /// <summary>
        /// Destroy function
        /// </summary>
        public void Destroy()
        {
            foreach (ARPlaneAnchorGameObject arpag in this.GetCurrentPlaneAnchors()) 
            {
                GameObject.Destroy(arpag.gameObject);
            }

            this.planeAnchorMap.Clear();
            this.UnsubscribeEvents();
        }

        /// <summary>
        /// GetCurrentPlaneAnchors function
        /// </summary>
        /// <returns>
        /// list of ARPlaneAnchorGameObject
        /// </returns> 
        public LinkedList<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors()
        {
            return this.planeAnchorMap.Values;
        }
    }
}