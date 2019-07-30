//  <copyright file="ScapeUtils.cs" company="Scape Technologies Limited">
//
//  ScapeUtils.cs
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
    using ScapeKitUnity;
    using UnityEngine;

    /// <summary>
    /// A class for geo conversions
    /// </summary>
    public class ScapeUtils 
    {
        /// <summary>
        /// return coordinates object as string with comma separator
        /// </summary>
        /// <param name="coords">
        /// a coordinates object
        /// </param>
        /// <returns>
        /// return a string
        /// </returns>
        public static string CoordinatesToString(LatLng coords) 
        {
            return coords.Latitude + ", " + coords.Longitude;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass _scapeUtils = null;
        private static AndroidJavaClass ProxyInstance
        {
            get
            {
                if(_scapeUtils == null) {

                    _scapeUtils = new AndroidJavaClass("com.scape.scapekit.ScapeUtils");
                }
                return _scapeUtils;
            }
        }

#elif UNITY_IPHONE || UNITY_EDITOR
        #if UNITY_IPHONE && !UNITY_EDITOR
            private const string LIBSCAPEBOX_LIBNAME = "__Internal";
        #elif UNITY_EDITOR_LINUX
            private const string LIBSCAPEBOX_LIBNAME = "scapebox_c";
        #elif UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
            private const string LIBSCAPEBOX_LIBNAME = "libscapebox_c";
        #endif
        [DllImport(LIBSCAPEBOX_LIBNAME)]
        public static extern long _cellIdForWgs(double latitude, double longitude, int s2CellLevel);
        [DllImport(LIBSCAPEBOX_LIBNAME)]
        public static extern double _metersBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);
        [DllImport(LIBSCAPEBOX_LIBNAME)]
        public static extern double _angleBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2);
        [DllImport(LIBSCAPEBOX_LIBNAME)]
        public static extern void _wgsToLocal(double latitude, double longitude, double altitude, long cellId, double[] result);
        [DllImport(LIBSCAPEBOX_LIBNAME)]
        public static extern void _localToWgs(double x, double y, double z, long cellId, double[] result);

#endif

        /// <summary>
        /// metersBetweenCoordinates given to coordinates returns distance between them in meters
        /// </summary>
        /// <param name="latitude1">
        /// latitude1 the first latitude parameter in degrees
        /// </param>
        /// <param name="longitude1">
        /// longitude1 the first longitude parameter in degrees
        /// </param>
        /// <param name="latitude2">
        /// latitude2 the second latitude parameter in degrees
        /// </param>
        /// <param name="longitude2">
        /// longitude2 the second longitude parameter in degrees
        /// </param>
        /// <returns>
        /// value in meters
        /// </returns>
        public static double MetersBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2) 
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return ScapeUtils.ProxyInstance.CallStatic<double>("metersBetweenCoordinates", latitude1, longitude1, latitude2, longitude2);
#else
            return _metersBetweenCoordinates(latitude1, longitude1, latitude2, longitude2);
#endif
        }

        /// <summary>
        /// angleBetweenCoordinates, the angular distance between 2 gps coords 
        /// </summary>
        /// <param name="latitude1">
        /// latitude1 the first latitude parameter in degrees
        /// </param>
        /// <param name="longitude1">
        /// longitude1 the first longitude parameter in degrees
        /// </param>
        /// <param name="latitude2">
        /// latitude2 the second latitude parameter in degrees
        /// </param>
        /// <param name="longitude2">
        /// longitude2 the second longitude parameter in degrees
        /// </param>
        /// <returns>
        /// value in degrees
        /// </returns>
        public static double AngleBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2) 
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return ScapeUtils.ProxyInstance.CallStatic<double>("angleBetweenCoordinates", latitude1, longitude1, latitude2, longitude2);
#else
            return _angleBetweenCoordinates(latitude1, longitude1, latitude2, longitude2);
#endif
        }

        /// <summary>
        /// WsgToLocal, given a LatLng coordinate and a s2cell id, returns Vector3.
        /// typically used by a GeoAnchor component to find it's position in the Unity scene. 
        /// </summary>
        /// <param name="latitude">
        /// latitude in degrees
        /// </param>
        /// <param name="longitude">
        /// longitude in degrees
        /// </param>
        /// <param name="altitude">
        /// altitude in meters
        /// </param>
        /// <param name="s2CellId">
        /// s2CellId long id
        /// </param>
        /// <returns>
        /// position for use in Unity scene
        /// </returns>
        public static Vector3 WgsToLocal(double latitude, double longitude, double altitude, long s2CellId) 
        {
            var vec3 = new Vector3();
#if UNITY_ANDROID && !UNITY_EDITOR
            using(AndroidJavaObject arrayJavaObj = ScapeUtils.ProxyInstance.CallStatic<AndroidJavaObject>("wgsToLocal", latitude, longitude, altitude, s2CellId))
            {
                using(AndroidJavaObject arrayJava = arrayJavaObj.Call<AndroidJavaObject>("toArray"))
                {
                    AndroidJavaObject[] javaDoubleArray = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(arrayJava.GetRawObject());
                    {
                        vec3.Set((float)javaDoubleArray[0].Call<double>("doubleValue"), 
                                 (float)javaDoubleArray[1].Call<double>("doubleValue"), 
                                 (float)javaDoubleArray[2].Call<double>("doubleValue")
                            );
                    }
                }
            }
#else
            double[] result = new double[3];
            _wgsToLocal(latitude, longitude, altitude, s2CellId, result);
            vec3.Set((float)result[0], (float)result[1], (float)result[2]);
#endif
            return vec3;
        }

        /// <summary>
        /// localToWsg, given a Unity position vector3 and s2 cell id, returns a LatLng coordinate.
        /// The Unity scene uses an S2 Cell's center to define it's origin.
        /// In this way by taking the world position of an object in the Unity scene
        /// and combining it with the Cell id defined in the GeoAnchorManager, the world coordinate of that
        /// position can be deduced.
        /// </summary>
        /// <param name="localVec3">
        /// local position in unity scene
        /// </param>
        /// <param name="s2CellId">
        /// s2CellId long id
        /// </param>
        /// <returns>
        /// GPS world coordinates
        /// </returns>
        public static LatLng LocalToWgs(Vector3 localVec3, long s2CellId) 
        {
            LatLng coords = new LatLng();
#if UNITY_ANDROID && !UNITY_EDITOR
            using(AndroidJavaObject arrayJavaObj = ScapeUtils.ProxyInstance.CallStatic<AndroidJavaObject>("localToWgs", (double)localVec3.x, (double)localVec3.y, (double)localVec3.z, s2CellId))
            {
                using(AndroidJavaObject arrayJava = arrayJavaObj.Call<AndroidJavaObject>("toArray"))
                {
                    AndroidJavaObject[] javaDoubleArray = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(arrayJava.GetRawObject());
                    {
                        List<double> doubleList = new List<double>();
                        foreach(AndroidJavaObject v in javaDoubleArray) 
                        {
                            doubleList.Add(v.Call<double>("doubleValue"));
                        }
                        coords = new LatLng() 
                        {
                            Latitude = doubleList[0],
                            Longitude = doubleList[1]
                        };
                    }
                }
            }

#else
            double[] result = new double[3];
            _localToWgs(localVec3.x, localVec3.y, localVec3.z, s2CellId, result);
            coords = new LatLng() 
            {
                Latitude = result[0],
                Longitude = result[1]
            };
#endif
            return coords;
        }

        /// <summary>
        /// cellIdForGps S2 cell id for GPS.
        /// S2 Cells are areas's on the Earth's surface.
        /// In a Scape integrated Unity scene the center of one such cell is always used as the
        /// scene's origin.
        /// This function is used by the GeoAnchorManager to ascertain the S2 Cell for the current scene. 
        /// </summary>
        /// <param name="latitude">
        /// latitude in degrees
        /// </param>
        /// <param name="longitude">
        /// longitude in degrees
        /// </param>
        /// <param name="s2CellLevel">
        /// An S2 Cell Level
        /// </param>
        /// <returns>
        /// ID of s2 cell containing GPS coordinate.
        /// </returns>
        public static long CellIdForWgs(double latitude, double longitude, int s2CellLevel) 
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return ScapeUtils.ProxyInstance.CallStatic<long>("cellIdForWgs", latitude, longitude, s2CellLevel);
#else
            return _cellIdForWgs(latitude, longitude, s2CellLevel);
#endif
        }
    }
}