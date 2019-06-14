using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ScapeKitUnity;

public class OpenDataFile : MonoBehaviour
{
    // data structures conform strictly to the data found in the
    // "streetdata.json" file found in the Resources folder.  
    [Serializable]
	public struct StreetObject 
    {
        public string Prefab;
		public LatLng LatLng;
	}

    [Serializable]
    public struct StreetData 
    {
        public StreetObject[] Objects;
    }

    void Start()
    {
        //open data file from resources folder
        var textAsset = Resources.Load<TextAsset>("streetdata");

        if (textAsset == null) 
        {
            Debug.Log("Failed to load streetdata");
            return;
        }

        //decode file content from json to object defined above
        StreetData streetData = JsonUtility.FromJson<StreetData>(textAsset.ToString());

        if(streetData.Objects.Length == 0) 
        {
            Debug.Log( "No objects parsed from json into specified data structure");
            return;
        }

        //iterate over the array of objects instantiating objects, adding GeoAnchor's to them.
        //The prefab named must be found in the "Resources" folder.
        foreach(StreetObject streetObject in streetData.Objects) 
        {
            Debug.Log( "Creating  " + streetObject.Prefab + " at " + ScapeUtils.CoordinatesToString(streetObject.LatLng));

            var loadedPrefab = Resources.Load(streetObject.Prefab, typeof(GameObject));
            if(loadedPrefab == null) {
                Debug.Log("Failed to load prefab");
                continue;
            }
        	var obj = Instantiate(loadedPrefab) as GameObject;

            // Once the GeoAnchor has been added it will register itself with the GeoAnchorManager
            // which should be present in the ScapeCamera prefab.
            // After the GeoAnchor starts it will deactivate the GameObject until the ScapeCamera
            // localizes. 
        	var anchor = obj.AddComponent(typeof(GeoAnchor)) as GeoAnchor;
        	anchor.LatLng = streetObject.LatLng;

            // we set scale here.
            // note setting position here would have no effect because the GeoAnchor will set 
            // the position again when the obj is activated. 
            obj.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
