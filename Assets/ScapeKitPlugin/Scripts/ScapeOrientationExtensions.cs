using UnityEngine;
using System.Collections;
using ScapeKitUnity;

namespace ScapeKitUnity
{

    public static class ScapeOrientationExtensions
    {
        public static Quaternion ToQuaternion(this ScapeOrientation o)
        {
            // Convert the right handed coordinates to left handed coordinates used by Unity (NED --> NWD)
            return new Quaternion((float)o.x,
                                  (float)-o.y,
                                  (float)o.z,
                                  (float)-o.w);
        }

        public static float Yaw(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Atan2(2.0f * (q.y * q.z + q.w * q.x), q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
        }

        public static float Pitch(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Asin(-2.0f * (q.x * q.z - q.w * q.y));
        }

        public static float Roll(this ScapeOrientation o)
        {
            var q = o.ToQuaternion();
            return Mathf.Atan2(2.0f * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);
        }

        public static float ToTrueScapeHeading(this ScapeOrientation q)
        {
            Quaternion globalOrientationCamToNwd = q.ToQuaternion();

            // Project the camera outward facing z-axis into the NWD-frame
            Vector3 unitZ = new Vector3(0, 0, 1);
            Vector3 cameraDirectionInNwd = globalOrientationCamToNwd * unitZ;

            // The heading is the angle between the projection of z onto the xy-plane in the NWD-frame
            float heading = Mathf.Atan2(cameraDirectionInNwd.y, cameraDirectionInNwd.x);

            // Heading is measured from [0, 2*pi], not [-pi, pi]
            if (heading < 0.0f)
                heading += 2 * Mathf.PI;

            // Convert from radians to degrees and return
            float trueHeading = heading * 180 / Mathf.PI;

            return trueHeading;
        }
    }
}
