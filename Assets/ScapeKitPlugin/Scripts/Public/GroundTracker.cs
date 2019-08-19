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
        /// <summary>
        /// needs to be called in order to check AR system for any further ground detection
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// returns the height of the first detected horizontal plane
        /// </summary>
        /// <param name="success">
        /// Only returns positive success once a horizintal plane has been detected
        /// </param>
        /// <returns>
        /// height in emters the deive is from the ground plane detected
        /// </returns> 
        public abstract float GetGroundHeight(out bool success);
    }
}