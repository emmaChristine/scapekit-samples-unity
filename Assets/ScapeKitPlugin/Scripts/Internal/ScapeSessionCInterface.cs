
namespace ScapeKitUnity
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using AOT;

    internal sealed class ScapeSessionCInterface : ScapeSession 
    {
        public override void GetMeasurements() {

        #if UNITY_ANDROID
            GetMeasurements(AcquireImageFromARCore());
        #else
            ScapeCInterface.citf_getMeasurements(this.scapeClient);
        #endif
        }
        
        public override void GetMeasurements(ScapeSession.ARImage image)
        {
            Debug.Log("ScapeSessionCInterface::GetMeasurements()");

            ScapeCInterface.citf_setYChannelPtr(this.scapeClient, image.YPixelBuffer, image.Width, image.Height);
            ScapeCInterface.citf_setCameraIntrinsics(this.scapeClient, 
                                    image.XFocalLength, 
                                    image.YFocalLength, 
                                    image.XPrincipalPoint, 
                                    image.YPrincipalPoint);

            ScapeCInterface.citf_getMeasurements(this.scapeClient);
        }

        public ScapeSession.ARImage AcquireImageFromARCore()
        {
            ScapeSession.ARImage arImage = new ARImage();

            using (GoogleARCore.CameraImageBytes image = GoogleARCore.Frame.CameraImage.AcquireCameraImageBytes())
            {
                if (image.IsAvailable)
                {
                    arImage.Width = image.Width;
                    arImage.Height = image.Height;
                    arImage.YPixelBuffer = image.Y;
                    arImage.XFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.x;
                    arImage.YFocalLength = GoogleARCore.Frame.CameraImage.ImageIntrinsics.FocalLength.y;
                    arImage.XPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.x;
                    arImage.YPrincipalPoint = GoogleARCore.Frame.CameraImage.ImageIntrinsics.PrincipalPoint.y;
                    arImage.IsAvailable = true;

                    Debug.Log("AcquireImageFromARCore()");
                }
            }
            return arImage;
        }

    	private IntPtr scapeClient;

    	internal ScapeSessionCInterface(IntPtr client) 
    	{
    		this.scapeClient = client;

    		ScapeCInterface.citf_setSessionCallbacks(this.scapeClient, 
    			onScapeMeasurementsRequestedNative,
    			onScapeSessionErrorNative,
    			onScapeMeasurementsUpdatedNative
    		);
    	}

        [MonoPInvokeCallback (typeof(ScapeCInterface.onScapeMeasurementsRequestedDelegate))]
        static void onScapeMeasurementsRequestedNative(int timestamp)
        {
            Debug.Log("onScapeMeasurementsRequestedNative");
        	ScapeSession.Instance.OnScapeMeasurementsRequested(timestamp);
        }

        [MonoPInvokeCallback (typeof(ScapeCInterface.onScapeMeasurementsUpdatedDelegate))]
        static void onScapeMeasurementsUpdatedNative(ScapeCInterface.scape_measurements sm)
        {
    		ScapeMeasurements scapeMeasurements;
			
			scapeMeasurements.Timestamp = sm.timestamp;
			scapeMeasurements.LatLng = new LatLng() {
				Latitude = sm.latitude, 
				Longitude = sm.longitude 
			};

			scapeMeasurements.Heading = sm.heading;
			scapeMeasurements.Orientation = new ScapeOrientation() {
				X = sm.orientationX, 
				Y = sm.orientationY, 
				Z = sm.orientationZ, 
				W = sm.orientationW
			};
			scapeMeasurements.RawHeightEstimate = sm.rawHeightEstimate;
			scapeMeasurements.ConfidenceScore = sm.confidenceScore;
			scapeMeasurements.MeasurementsStatus = (ScapeMeasurementStatus)sm.measurementsStatus;

            Debug.Log("onScapeMeasurementsUpdatedNative");
            ScapeSession.Instance.OnScapeMeasurementsEvent(scapeMeasurements);
        }

        [MonoPInvokeCallback (typeof(ScapeCInterface.onScapeSessionErrorDelegate))]
        static void onScapeSessionErrorNative(int errorStatus, [MarshalAs(UnmanagedType.LPStr)] string errorMessage)
        {
        	Debug.Log("onScapeSessionError " + errorMessage);

    		ScapeSession.Instance.OnScapeSessionErrorEvent(new ScapeSessionError() {
    			State = (ScapeSessionState)errorStatus, 
    			Message = errorMessage
    		});
        }
    }
}