using UnityEngine;

namespace ScapeKitUnity
{
	public class GeoAnchor : MonoBehaviour
	{
		public double Longitude;
		public double Latitude;
		public double MaxDistance = 1000.0;

		//this Vector2 corresponds to the x,z unity position corrected to be relatively correct to Unity's origin
		//this is only true after a succesful scape measurement.
		protected Vector2 _sceenPos;
		public Vector2 ScenePos
		{
			protected set { _sceenPos = value; }
			get { return _sceenPos; }
		}

		protected Coordinates _worldCoordinates;
		public Coordinates WorldCoordinates
		{
			set { _worldCoordinates = value; }
			get { return _worldCoordinates; }
		}

		public bool WithinMaxDistance() 
		{
			return ScenePos.magnitude < MaxDistance;
		}

		private bool needsUpdate = false;

		private string gameObjectName;

		//The OriginEvent function has been connected to the GeoWorldRoot's GeoOriginEvent action
		//which is triggered when the main camera receives a successful scape measurement. 
		//The GeoWorldRoot passes the WorldCoordinates for the origin of the Unity scene. 
		//The UnityRelativePosition function calculates the position of this GameObject within the Unity scene
		//relative to GeoWorldRoot's world location.
		public void OriginEvent(Coordinates SceneOriginCoordinates) {

			ScenePos = GeoConversions.UnityRelativePosition(SceneOriginCoordinates, WorldCoordinates);

			ScapeLogging.Log(message: "GeoAnchor::OriginEvent() " + gameObjectName + " ScenePos = " + ScenePos.ToString());

			needsUpdate = true;
		}

		//Upon Start up the GeoAnchor creates it's WorldCoodinate object based on it's public
		//Longitude and Latitude variables (these must be initalized to some world coordinate).
		//It then register's itself with the GeoWorlRoot singleton which manages 
		//much of it's lifetime going forward. 
		//Usually the GeoWorldRoot will immediately deactivate the gameobject untill a scape measurement
		//arrives and it's place in the local Unity coordinate system can be decided.
		void Start() {		

			gameObjectName = this.gameObject.name;
			
			WorldCoordinates = new Coordinates(){longitude = Longitude, latitude = Latitude};
			
			GeoWorldRoot.Instance.RegisterGeoAnchor(this);

			ScapeLogging.Log(message: "GeoAnchor::Start() WorldCoords = " + GeoConversions.CoordinatesToString(WorldCoordinates));
		}

		void Update() {

			if(needsUpdate) 
			{
				this.gameObject.transform.localPosition = new Vector3(ScenePos.x, 0.0f, ScenePos.y);

				needsUpdate = false;
			}
		}
	} 
}