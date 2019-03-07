using UnityEngine;
using UnityEngine.XR;

namespace ScapeKitUnity
{

    public abstract class ScapeCameraComponent : MonoBehaviour
    {
    	public Camera TheCamera;

    	protected Vector3 PositionAtScapeMeasurements;
    	protected Vector3 RotationAtScapeMeasurements;

        public float CameraHeadingOffset = 0.0f;
        public float CameraHeight = 0.0f;

        private float ScapeHeading = -1.0f;
    	private float ScapeDirectionFix = 0.0f;

        private void initScape()
        {
            ScapeClient.Instance.WithResApiKey().WithDebugSupport(true).StartClient();

            ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
        }

        public abstract void GetMeasurements();

        public abstract void UpdateCameraFromAR();

        public void Awake() { 

            initScape();

            if(!TheCamera) {
                TheCamera = Camera.main;
            }
        }

        public void Update()
        {
            UpdateCameraFromAR();
        }

        public Quaternion GetScapeHeading() 
        {
            return Quaternion.AngleAxis(ScapeDirectionFix+CameraHeadingOffset,  Vector3.up);
        }

        public Vector3 GetScapePosition() 
        {
            return new Vector3(0.0f, CameraHeight, 0.0f);
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