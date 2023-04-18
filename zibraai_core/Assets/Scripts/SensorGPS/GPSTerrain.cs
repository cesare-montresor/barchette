using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Univr.Barchette.Sensors.GPS
{
    public class GPSTerrain : MonoBehaviour
    {
        [Range(-90.0f, 90.0f)]
        public float longitudeMin;
        [Range(-90.0f, 90.0f)]
        public float longitudeMax;

        [Range(-90.0f, 90.0f)]
        public float latitudeMin;
        [Range(-90.0f, 90.0f)]
        public float latitudeMax;

        [Range(-1000.0f, 1000.0f)]
        public float altitudeMin;
        [Range(-1000.0f, 1000.0f)]
        public float altitudeMax;
        
        public Terrain terrain;

        public float DistanceTo(GPSTerrain other)
        {
            return DistanceTo(other.terrain.transform.position);
        }
        public float DistanceTo(Vector3 other) {
            var pos = terrain.transform.position;
            return Vector3.Distance(other, pos);
        }

        void Start()
        {
            terrain = GetComponent<Terrain>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public struct LatLng
    {
        public float longitude, latitude, altitude;
    }
    public struct LatLngAlt
    {
        public float longitude, latitude, altitude;
    }
    public struct CoordsRange
    {
        public float min, max;
    }


}