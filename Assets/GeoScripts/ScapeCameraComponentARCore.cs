using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using GoogleARCoreInternal;
#endif

namespace ScapeKitUnity
{

#if UNITY_ANDROID && !UNITY_EDITOR
    public class ScapeCameraComponentARCore : ScapeCameraComponent
    {
    	void Awake() 
    	{
    		base.Awake();
    	}

    	void Update()
    	{
    		base.Update();
    	}

        public override Quaternion GetARRotation()
        {
        	return GoogleARCore.Frame.Pose.rotation;
        }
        public override Vector3 GetARPosition()
        {
        	return GoogleARCore.Frame.Pose.position;
        }

    	public override void UpdateCameraFromAR() 
    	{
            TheCamera.transform.localPosition = GoogleARCore.Frame.Pose.position + new Vector3(0.0f, CameraHeight, 0.0f);
            TheCamera.transform.localRotation = GoogleARCore.Frame.Pose.rotation;
    	}

    	public override void GetMeasurements() 
    	{
            PositionAtScapeMeasurements = new Vector3(GoogleARCore.Frame.Pose.position.x, 
                                                        GoogleARCore.Frame.Pose.position.y,
                                                        GoogleARCore.Frame.Pose.position.z);

            RotationAtScapeMeasurements = new Quaternion(GoogleARCore.Frame.Pose.rotation.x, 
                                                        GoogleARCore.Frame.Pose.rotation.y, 
                                                        GoogleARCore.Frame.Pose.rotation.z, 
                                                        GoogleARCore.Frame.Pose.rotation.w).eulerAngles;
    	}
    }
#endif
}