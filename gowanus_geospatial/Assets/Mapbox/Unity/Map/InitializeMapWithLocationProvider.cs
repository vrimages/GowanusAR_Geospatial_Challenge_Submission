namespace Mapbox.Unity.Map
{
	using System.Collections;
	using Mapbox.Unity.Location;
	using UnityEngine;

	public class InitializeMapWithLocationProvider : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		ILocationProvider _locationProvider;
    
		private void Awake()
		{
			// Prevent double initialization of the map. 
			_map.InitializeOnStart = false;

			// initialize map here instead of with Start function
			//_map.Initialize(_map.CenterLatitudeLongitude, _map.AbsoluteZoom);
			//Debug.Log(_map.CenterLatitudeLongitude);
            Debug.Log(_map.Options.locationOptions.latitudeLongitude);
			Debug.Log(_map._options.locationOptions.latitudeLongitude);
        }

        /*void Start()
        {
            
        }*/

        
		protected virtual IEnumerator Start()
		{
			yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}

		void LocationProvider_OnLocationUpdated(Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_map.Initialize(location.LatitudeLongitude, _map.AbsoluteZoom);
		}
    }
}
