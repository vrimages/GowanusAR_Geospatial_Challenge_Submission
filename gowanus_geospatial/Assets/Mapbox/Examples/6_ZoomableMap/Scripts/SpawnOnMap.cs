namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		//[SerializeField]
		//float _spawnScale = 100f;
		

		[SerializeField]
		GameObject _markerPrefab;

		[SerializeField]
		List<GameObject> _markerPrefabs;

		List<GameObject> _spawnedObjects;

		void Start()
		{
			/*
			_spawnedObjects = new List<GameObject>();

			_locations = new Vector2d[1];

			var locationString = _locationStrings[7];
			_locations[0] = Conversions.StringToLatLon(locationString);

			var instance = Instantiate(_markerPrefabs[7]);
            instance.transform.localPosition = _map.GeoToWorldPosition(_locations[0], true);

			_spawnedObjects.Add(instance);
			*/

			//SpawnMarkers();
		}

		public void SpawnMarkers(){
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Length; i++)
			{
				if(i!=10){
					var locationString = _locationStrings[i];
					_locations[i] = Conversions.StringToLatLon(locationString);
					//var instance = Instantiate(_markerPrefab);
					var instance = Instantiate(_markerPrefabs[i]);
					instance.tag = (i).ToString();
					instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
					//instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
					_spawnedObjects.Add(instance);
				}
				
			}
		}

		private void Update()
		{
			if(_spawnedObjects != null){
				int count = _spawnedObjects.Count;
				for (int i = 0; i < count; i++)
				{
					var spawnedObject = _spawnedObjects[i];
					var location = _locations[i];
					if(spawnedObject!=null)
						spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
					//spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				}
			}
			
		}
	}
}