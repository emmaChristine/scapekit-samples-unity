//  <copyright file="GroundTrackerARCore.cs" company="Scape Technologies Limited">
//
//  GroundTrackerARCore.cs
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

	/// <summary>
	/// A class for monitoring plane tracking using ar platforms
	/// </summary>
	public class GroundTrackerARCore : GroundTracker
	{

		private bool haveArPlanes = false;
		private float groundHeight = 0.0f;

		public override void Update() 
		{
			if(haveArPlanes) 
			{
				return;
			} 

       		List<DetectedPlane> newPlanes = new List<DetectedPlane>();

            Session.GetTrackables<DetectedPlane>(newPlanes, TrackableQueryFilter.New);

            if(newPlanes.Count > 0) 
            {
            	groundHeight = newPlanes[0].CenterPose.position.y;            
            	ScapeLogging.LogDebug(message: "GroundTracker::Update() Found ground at " + groundHeight);
            	haveArPlanes = true;
            }
		}

		public override float GetGroundHeight(out bool success)
		{
			success = haveArPlanes;

			return groundHeight;
		} 
	}
}
