using UnityEditor;
using UnityEngine;
using System;

namespace ScapeKitUnity
{
    public static class GeoConversions
    {
        public static Vector2 UnityRelativePosition(Coordinates OriginCoords, Coordinates ObjectCoordinates)
        {
            return VectorFromCoordinates(new Coordinates() 
            {
                longitude = ObjectCoordinates.longitude - OriginCoords.longitude,
                latitude = ObjectCoordinates.latitude - OriginCoords.latitude
            });
        }
    	public static Quaternion UnityQuatFromScapeOrientation(ScapeOrientation scape) 
        {
    		return new Quaternion((float)scape.x, (float)scape.y, (float)scape.z, (float)scape.w);
    	}

    	public static Vector2 VectorFromCoordinates(Coordinates coords) 
        {
            return new Vector2((float)LonToX(coords.longitude), (float)LatToY(coords.latitude));
    	}

    	public static Coordinates CoordinatesFromVector(Vector2 pos) 
        {
    		return new Coordinates() {longitude = XToLon(pos.x), latitude = YToLat(pos.y)};
    	}
        
        static double EarthRadiusForEPSG3857 = 6378137.0;

 		public static double LonToX(double lon) 
        {
            return EarthRadiusForEPSG3857 * DegreesToRadians(lon);
        }

        public static double LatToY(double lat) 
        { 
            return EarthRadiusForEPSG3857 * Math.Log(Math.Tan(Math.PI / 4 + DegreesToRadians(lat) / 2));
        }

        public static double XToLon(double x) 
        {
            return RadiansToDegrees(x) / EarthRadiusForEPSG3857;
        }

        public static double YToLat(double y) 
        {
            return RadiansToDegrees(2 * Math.Atan(Math.Exp(y / EarthRadiusForEPSG3857)) - Math.PI / 2);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static string CoordinatesToString(Coordinates coords) 
        {
            return "lon=" + coords.longitude + " lat=" + coords.latitude;
        }
    }
}