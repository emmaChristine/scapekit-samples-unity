using UnityEngine;

namespace ScapeKitUnity
{
	public class GeoAnchor : MonoBehaviour
	{
		public double Longitude;
		public double Latitude;
		public double MaxDistance = 1000.0;

		private Vector2 WorldPos;
		private Vector2 ScenePos;
		private Coordinates WorldCoordinates;
		private bool needsUpdate = false;
		private bool isActive = false;

		private string gameObjectName;

		void OriginEvent(Coordinates SceneOriginCoordinates) {
			
			ScapeLogging.Log(message: "OriginEvent() " + gameObjectName);

			Vector2 SceneOrigin = GeoConversions.VectorFromCoordinates(SceneOriginCoordinates);

			ScenePos = WorldPos - SceneOrigin;

			ScapeLogging.Log(message: "OriginEvent() " + gameObjectName + " ScenePos = " + ScenePos.ToString());
			ScapeLogging.Log(message: "OriginEvent() " + gameObjectName + " WorldCoords = " + GeoConversions.CoordinatesToString(WorldCoordinates));

			if(ScenePos.magnitude < MaxDistance) {

				needsUpdate = true;
				isActive = true;
			}
			else {
				ScapeLogging.Log(message: "OriginEvent() "+ gameObjectName +" beyond max distance (" + ScenePos.magnitude + ")");
				ScenePos = new Vector3(0.0f, -10000.0f, 0.0f);
				needsUpdate = true;
			}
		}

		void hide() 
		{
			this.gameObject.transform.position = new Vector3(0.0f, -10000.0f, 0.0f);	
		}

		void Awake() {			

			hide();

			gameObjectName = this.gameObject.name;

			ScapeLogging.Log(message: "GeoAnchor::Awake " + this.gameObject.name);

			GeoWorldRoot.GetInstance().RegisterGeoEvent(this.OriginEvent);
			
			WorldCoordinates = new Coordinates{longitude = Longitude, latitude = Latitude};
			WorldPos = GeoConversions.VectorFromCoordinates(WorldCoordinates);
		}

		void Update() {

			if(needsUpdate) {

				this.gameObject.transform.position = new Vector3(ScenePos.x, 0.0f, ScenePos.y);	

				ScapeLogging.Log(message: "GameObject::Update() " + gameObjectName + " @ " + this.gameObject.transform.position.ToString());	
	
				needsUpdate = false;

			}
		}
	} 
}