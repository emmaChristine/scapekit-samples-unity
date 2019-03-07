using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace ScapeKitUnity
{

    public class ScapeCameraComponentARKit : ScapeCameraComponent 
    {
        private UnityARSessionNativeInterface m_session;
        private Material savedClearMaterial;
        private bool sessionStarted = false;

        public ARKitWorldTrackingSessionConfiguration sessionConfiguration
        {
            get
            {
                ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration ();
                config.planeDetection = UnityARPlaneDetection.Horizontal;
                config.alignment = UnityARAlignment.UnityARAlignmentGravity;
                config.getPointCloudData = false;
                config.enableLightEstimation = false;
                config.enableAutoFocus = true;
                config.maximumNumberOfTrackedImages = 0;
                config.environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;

                return config;
            }
        }

        void Awake() 
        {
            base.Awake();
        }

        void Update()
        {
            base.Update();
        }

        // Use this for initialization
        void Start () {

            m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            Application.targetFrameRate = 60;
            
            var config = sessionConfiguration;
            if (config.IsSupported) {
                m_session.RunWithConfig (config);
                UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
            }

            if (TheCamera == null) {
                TheCamera = Camera.main;
            }
        }

        void OnDestroy()
        {
            m_session.Pause();
        }

        void FirstFrameUpdate(UnityARCamera cam)
        {
            sessionStarted = true;
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
        }

        public void SetCamera(Camera newCamera)
        {
            if (TheCamera != null) {
                UnityARVideo oldARVideo = TheCamera.gameObject.GetComponent<UnityARVideo> ();
                if (oldARVideo != null) {
                    savedClearMaterial = oldARVideo.m_ClearMaterial;
                    Destroy (oldARVideo);
                }
            }
            SetupNewCamera (newCamera);
        }

        private void SetupNewCamera(Camera newCamera)
        {
            TheCamera = newCamera;

            if (TheCamera != null) {
                UnityARVideo unityARVideo = TheCamera.gameObject.GetComponent<UnityARVideo> ();
                if (unityARVideo != null) {
                    savedClearMaterial = unityARVideo.m_ClearMaterial;
                    Destroy (unityARVideo);
                }
                unityARVideo = TheCamera.gameObject.AddComponent<UnityARVideo> ();
                unityARVideo.m_ClearMaterial = savedClearMaterial;
            }
        }

        // Update is called once per frame

        public override void UpdateCameraFromAR()
        {
            if (TheCamera != null && sessionStarted)
            {
                // JUST WORKS!
                Matrix4x4 matrix = m_session.GetCameraPose();
                TheCamera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix) + GetScapePosition();
                TheCamera.transform.localRotation = GetScapeHeading() * UnityARMatrixOps.GetRotation(matrix);

                TheCamera.projectionMatrix = m_session.GetCameraProjection ();
            }
        }

        public override void GetMeasurements() 
        {   
            if(sessionStarted) {

                Matrix4x4 matrix = m_session.GetCameraPose();
                PositionAtScapeMeasurements = UnityARMatrixOps.GetPosition(matrix);
                RotationAtScapeMeasurements = UnityARMatrixOps.GetRotation(matrix).eulerAngles;
            } 
        }
    }

}
