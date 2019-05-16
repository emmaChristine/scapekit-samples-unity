using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ScapeKitUnity;
using Newtonsoft.Json;

public class OpenDataFile : MonoBehaviour
{
    [Serializable]
	public struct StreetObject {
        public string Prefab;
		public LatLng LatLng;
	}

    [Serializable]
    public struct StreetData {
        public StreetObject[] Objects;
    }

	StreetData streetData;

    void Start()
    {
    	Debug.Log( "OpenDataFile::Start()");

        var textAsset = Resources.Load<TextAsset>("streetdata");

        if (textAsset == null) {
            Debug.Log("Failed to load streetdata");
            return;
        }

        string data = textAsset.ToString();

        streetData = JsonUtility.FromJson<StreetData>(data);

        if(streetData.Objects.Length == 0) {
            Debug.Log( "No objects parsed from json into specified data structure");
            return;
        }

        Debug.Log( "StreetObjects size = " + streetData.Objects.Length);

        foreach(StreetObject streetObject in streetData.Objects) {

            Debug.Log( "Creating  " + streetObject.Prefab + " at " + ScapeUtils.CoordinatesToString(streetObject.LatLng));

            var loadedPrefab = Resources.Load(streetObject.Prefab, typeof(GameObject));
            if(loadedPrefab == null) {
                Debug.Log("Failed to load prefab");
                continue;
            }
        	var obj = Instantiate(loadedPrefab) as GameObject;

        	var anchor = obj.AddComponent(typeof(GeoAnchor)) as GeoAnchor;
        	anchor.Longitude = streetObject.LatLng.Longitude;
        	anchor.Latitude = streetObject.LatLng.Latitude;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
