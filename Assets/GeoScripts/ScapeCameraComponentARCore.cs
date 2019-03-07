using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using GoogleARCoreInternal;
#endif

namespace ScapeKitUnity
{

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
    	public override void UpdateCameraFromAR() 
    	{
#if UNITY_ANDROID && !UNITY_EDITOR
            TheCamera.transform.localPosition = GoogleARCore.Frame.Pose.position + new Vector3(0.0f, CameraHeight, 0.0f);
            TheCamera.transform.localRotation = GetScapeHeading() * GoogleARCore.Frame.Pose.rotation;
#endif
    	}

    	public override void GetMeasurements() 
    	{
#if UNITY_ANDROID && !UNITY_EDITOR
            PositionAtScapeMeasurements = new Vector3(GoogleARCore.Frame.Pose.position.x, 
                                                        GoogleARCore.Frame.Pose.position.y,
                                                        GoogleARCore.Frame.Pose.position.z);

            RotationAtScapeMeasurements = new Quaternion(GoogleARCore.Frame.Pose.rotation.x, 
                                                        GoogleARCore.Frame.Pose.rotation.y, 
                                                        GoogleARCore.Frame.Pose.rotation.z, 
                                                        GoogleARCore.Frame.Pose.rotation.w).eulerAngles;
#endif
    	}
    }
}