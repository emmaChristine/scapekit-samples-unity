using System;

namespace ScapeKitUnity
{
	class GeoWorldRoot 
	{
		internal Action<Coordinates> GeoOriginEvent;

		internal static GeoWorldRoot _instance = null;

		public static GeoWorldRoot GetInstance() {

			if(_instance == null) {
				_instance = new GeoWorldRoot();
			}
			return _instance;
		}

		public void SetWorldOrigin(Coordinates coordinates) {
			if(GeoOriginEvent != null) {
				GeoOriginEvent(coordinates);
			}
		}

		public void RegisterGeoEvent(Action<Coordinates> GeoEvent)
		{
			GeoOriginEvent += GeoEvent;
		}
	}
}