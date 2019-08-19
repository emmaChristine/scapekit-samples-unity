//  <copyright file="DebugSessionCInterface.cs" company="Scape Technologies Limited">
//
//  DebugSessionCInterface.cs
//  ScapeKitUnity
//
//  Created by nick on 1/5/2019.
//  Copyright © 2019 Scape Technologies Limited. All rights reserved.
//  </copyright>

namespace ScapeKitUnity
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class DebugSessionCInterface : DebugSession 
    {
        private IntPtr nativePtr = IntPtr.Zero;

        public DebugSessionCInterface(IntPtr np) 
        {
            nativePtr = np;
        }

        public override void SetLogConfig(LogLevel level, LogOutput output) 
        {
            ScapeCInterface.citf_setLogConfig(nativePtr, (int)level, (int)output);
        }
        public override void MockGPSCoordinates(double latitude, double longitude) 
        {
            ScapeCInterface.citf_mockGPSCoordinates(nativePtr, latitude, longitude);
        }
        public override void SaveImages(bool save)
        {
            ScapeCInterface.citf_saveImages(nativePtr, save);
        }

    }
}