using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

namespace ScapeKitUnity
{

    public abstract class ScapeCameraComponent : MonoBehaviour
    {
        public Camera  TheCamera;
        public Text    TextLogging;

        public bool UseScapeLocation = true;
        public bool UseDeviceLocation = false;

        protected Vector3 PositionAtScapeMeasurements;
        protected Vector3 RotationAtScapeMeasurements;

        protected Coordinates OriginCoordinates;

        public float CameraHeadingOffset = 0.0f;
        public float CameraHeight = 0.0f;

        private float ScapeHeading = -1.0f;
        private float ScapeDirectionFix = 0.0f;

        private bool updateRoot = false;

        private void initScape()
        {
            ScapeClient.Instance.WithResApiKey().WithDebugSupport(true).StartClient();

            if(UseScapeLocation) 
            {
                ScapeClient.Instance.ScapeSession.ScapeMeasurementsEvent += OnScapeMeasurementsEvent;
            }
            if(UseDeviceLocation) 
            {
                ScapeClient.Instance.ScapeSession.DeviceLocationMeasurementsEvent += OnDeviceLocationMeasurementsEvent;
            }
        }

        public abstract void GetMeasurements();

        public abstract void UpdateCameraFromAR();

        public abstract Quaternion GetARRotation();
        public abstract Vector3 GetARPosition();

        public void Awake() 
        { 
            initScape();

            if(!TheCamera) {
                TheCamera = Camera.main;
            }
        }
        public void Update()
        {
            UpdateCameraFromAR();
            UpdateDisplay();
            UpdateRoot();
        }

        public void UpdateDisplay() 
        {
            if(TextLogging != null)
            {
                TextLogging.text = "ARRotation = " + GetARRotation().eulerAngles.y + 
                                    "\nScapeFixAngle = " + ScapeDirectionFix + 
                                    "\nFinalAngle = " + ScapeDirectionFix + (GetARRotation()).eulerAngles.y;
            }
        }

        public Vector3 GetScapePosition() 
        {
            return new Vector3(0.0f, CameraHeight, 0.0f);
        }

        void UpdateRoot() {

            if(updateRoot) 
            {
                ScapeLogging.Log(message: "ScapeCameraComponent::UpdateRoot() ");
                GeoWorldRoot.Instance.SetWorldOrigin(OriginCoordinates, -ScapeDirectionFix + CameraHeadingOffset);
             
                updateRoot = false;   
            }
        }

        void SynchronizeARCamera(Coordinates scapeCoordinates, float heading) 
        {
            Coordinates LocalCoordinates = GeoConversions.CoordinatesFromVector(new Vector2(PositionAtScapeMeasurements.x, PositionAtScapeMeasurements.z));
            OriginCoordinates = new Coordinates() {
                longitude = scapeCoordinates.longitude - LocalCoordinates.longitude,
                latitude = scapeCoordinates.latitude - LocalCoordinates.latitude
            };

            ScapeLogging.Log(message: "SynchronizeARCamera() OriginCoordinates = " + GeoConversions.CoordinatesToString(OriginCoordinates));

            ScapeHeading = heading;

            ScapeDirectionFix = ScapeHeading - RotationAtScapeMeasurements.y;
            ScapeLogging.Log(message: "SynchronizeARCamera() ScapeDirectionFixYAngle = " + ScapeDirectionFix);

            updateRoot = true;
        }

        void OnScapeMeasurementsEvent(ScapeMeasurements scapeMeasurements)
        {
            if(scapeMeasurements.measurementsStatus == ScapeMeasurementStatus.ResultsFound) 
            {
                SynchronizeARCamera(scapeMeasurements.coordinates, (float)scapeMeasurements.heading);
            }   
        }
        void OnDeviceLocationMeasurementsEvent(LocationMeasurements locationMeasurements)
        {   
            ScapeLogging.Log(message: "ScapeCameraComponent::OnDeviceLocationMeasurementsEvent"); 
            SynchronizeARCamera(locationMeasurements.coordinates, (float)locationMeasurements.heading);
        }
    }
}