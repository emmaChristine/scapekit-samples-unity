using System;
using UnityEngine;

namespace ScapeKitUnity
{
	class GeoWorldRoot : MonoBehaviourSingleton<GeoWorldRoot>
	{
		private Action<Coordinates> GeoOriginEvent;

		//after a scape measurement has come in the OriginCoordinates of this GeoWorldRoot
		//are the world coordinates of the origin of the Unity scene.
		protected Coordinates _originCoordinates;
		public Coordinates OriginCoordinates
		{
			protected set { _originCoordinates = value; }
			get { return _originCoordinates; }
		}

		public static GeoWorldRoot Instance
		{
			get
            {
                return BehaviourInstance as GeoWorldRoot;
            }
		}

		private bool isInstantiated = false;
		private bool updateMain = false;

		void Update() 
		{
			//upon receiving the SetWorldOrigin function from the camera/scape measurements, at the next update
			//we decide to activate the GeoAnchored gameobjects
			if(updateMain)
			{	
				foreach(Transform child in this.gameObject.transform) 
				{
					var geoAnchor = child.GetComponent<GeoAnchor>();
					if(geoAnchor != null) 
					{
						child.gameObject.SetActive(geoAnchor.WithinMaxDistance());
					}
				}
				updateMain = false;
			}
		}

		//SetWorldOrigin is called by the main camera when a successful scape meaasurements event happens.
		//The Coordinates passed in are the Geo Location of Unity's origin.
		public void SetWorldOrigin(Coordinates coordinates) {

			OriginCoordinates = coordinates;

			isInstantiated = true;

			if(GeoOriginEvent != null) 
			{
				GeoOriginEvent(coordinates);
			}
			updateMain = true;
		}

		//Each GeoAnchored GameObject registers itself to the GeoWorldRoot singleton.
		//This sets the GeoWorldRoot's gameObject as it's parent transform and registers it's "OriginEvent" function to 
		//be called when a successful scape measurenment comes in.
		public void RegisterGeoAnchor(GeoAnchor geoAnchor)
		{
			geoAnchor.transform.SetParent(this.gameObject.transform, false);

			//append the GeoAnchor's OriginEvent function to the GeoOriginEvent action
			GeoOriginEvent += geoAnchor.OriginEvent;

			if(!isInstantiated) 
			{
				geoAnchor.gameObject.SetActive(false);
			}
			else 
			{
				geoAnchor.OriginEvent(OriginCoordinates);
				geoAnchor.gameObject.SetActive(geoAnchor.WithinMaxDistance());
			}

			ScapeLogging.Log(message: "GeoWorldRoot::RegisterGeoAnchor() " + geoAnchor.gameObject.name);
		}
	}
} 