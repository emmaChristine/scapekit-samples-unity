//  <copyright file="GroundTrackerARKit.cs" company="Scape Technologies Limited">
//
//  GroundTrackerARKit.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>


namespace ScapeKitUnity
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.XR.iOS;

	/// <summary>
	/// A class for monitoring plane tracking using arkit
	/// </summary>
	public class GroundTrackerARKit : GroundTracker
	{
		private bool haveArPlanes = false;
		private float groundHeight = -1.6f;

        private UnityARAnchorManager unityARAnchorManager;

		public override void Update()
		{
			if(haveArPlanes)
			{
				return;
			}
			if(unityARAnchorManager == null)
			{
            	unityARAnchorManager = new UnityARAnchorManager();
			}

			IEnumerable<ARPlaneAnchorGameObject> planeAnchors = unityARAnchorManager.GetCurrentPlaneAnchors ();
			foreach(var plane in planeAnchors) 
			{
				if(plane.planeAnchor.alignment == ARPlaneAnchorAlignment.ARPlaneAnchorAlignmentHorizontal)
				{
					groundHeight = plane.gameObject.transform.position.y;
					haveArPlanes = true;
	            	ScapeLogging.LogDebug(message: "GroundTracker::Update() Found ground at " + groundHeight);
	            	break;
				}
			}
		}
		public override float GetGroundHeight(out bool success)
		{
			success = haveArPlanes;
			return groundHeight;
		}
	}
}
