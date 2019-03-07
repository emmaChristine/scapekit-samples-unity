using UnityEngine;
using UnityEngine.UI; 

#if UNITY_ANDROID && !UNITY_EDITOR
using GoogleARCoreInternal;
#endif
using UnityEngine.XR;

namespace ScapeKitUnity
{

    public class ScapeCameraComponent : MonoBehaviour
    {
    	public Camera TheCamera;

    	private Vector3 PositionAtScapeMeasurements;
    	private Vector3 RotationAtScapeMeasurements;

        public float CameraHeadingOffset = 0.0f;
        public float CameraHeight = 0.0f;

        private float ScapeHeading = -1.0f;
    	private float ScapeDirectionFix = 0.0f;

        private void initScape()
        {
            ScapeClient.Instance.WithResApiKey().WithDebugSupport(true).StartClient();

            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
        }

        public void GetMeasurements()
        {

            PositionAtScapeMeasurements = new Vector3(GoogleARCore.Frame.Pose.position.x, 
                                                        GoogleARCore.Frame.Pose.position.y,
                                                        GoogleARCore.Frame.Pose.position.z);

            RotationAtScapeMeasurements = new Quaternion(GoogleARCore.Frame.Pose.rotation.x, 
                                                        GoogleARCore.Frame.Pose.rotation.y, 
                                                        GoogleARCore.Frame.Pose.rotation.z, 
                                                        GoogleARCore.Frame.Pose.rotation.w).eulerAngles;

            ScapeLogging.Log(message: "RotationAtScapeMeasurements.eulerAngles.y = " + RotationAtScapeMeasurements.y);
         }

        void Awake() { 

            initScape();

        	if(!TheCamera) {
        		TheCamera = Camera.main;
        	}
        }

    	void Update()
    	{
    		UpdateCameraFromARCore();
    	}

        void UpdateCameraFromARCore() {
#if UNITY_ANDROID && !UNITY_EDITOR
            TheCamera.transform.localPosition = GoogleARCore.Frame.Pose.position + new Vector3(0.0f, CameraHeight, 0.0f);
            TheCamera.transform.localRotation = Quaternion.AngleAxis(ScapeDirectionFix+CameraHeadingOffset,  Vector3.up) * GoogleARCore.Frame.Pose.rotation;
#endif
        }
    	void SynchronizeARCamera(ScapeMeasurements scapeMeasurements) 
    	{
    		Coordinates LocalCoordinates = GeoConversions.CoordinatesFromVector(new Vector2(PositionAtScapeMeasurements.x, PositionAtScapeMeasurements.z));
    		Coordinates OriginCoordinates = new Coordinates() {
    			longitude = scapeMeasurements.coordinates.longitude - LocalCoordinates.longitude,
    			latitude = scapeMeasurements.coordinates.latitude - LocalCoordinates.latitude
    		};

    		ScapeLogging.Log(message: "SynchronizeARCamera() origincoords = " + GeoConversions.CoordinatesToString(OriginCoordinates));

            GeoWorldRoot.GetInstance().SetWorldOrigin(OriginCoordinates);

            ScapeHeading = (float)scapeMeasurements.heading;

    		ScapeDirectionFix = ScapeHeading - RotationAtScapeMeasurements.y;
    		ScapeLogging.Log(message: "SynchronizeARCamera() ScapeDirectionFixYAngle = " + ScapeDirectionFix);
    	}

        void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
        	if(scapeMeasurements.measurementsStatus == ScapeMeasurementStatus.ResultsFound) 
        	{
				SynchronizeARCamera(scapeMeasurements);
        	}	
        }
    }
}