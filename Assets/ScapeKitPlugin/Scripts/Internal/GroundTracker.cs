//  <copyright file="GroundTracker.cs" company="Scape Technologies Limited">
//
//  GroundTracker.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>


namespace ScapeKitUnity
{
    using System.Collections.Generic;
    using UnityEngine;
    
	/// <summary>
	/// A class for monitoring plane tracking using ar platforms
	/// </summary>
	public abstract class GroundTracker 
	{
		public abstract void update();

		public abstract float GetGroundHeight(out bool success);
	}
}