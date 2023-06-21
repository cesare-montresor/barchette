using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Univr.Barchette.Sensors.GPS
{                                     
    

    public class GPSBeacon : MonoBehaviour
    {
        private static List<GPSBeacon> m_BeaconList = new();
        private static void RegisterBeacon(GPSBeacon beacon)
        {
            m_BeaconList.Add(beacon);
            m_BeaconList.ForEach(b => { b.UpdateRatios(); });
        }

        private static void UnregisterBeacon(GPSBeacon beacon)
        {
            m_BeaconList.Remove(beacon);
            m_BeaconList.ForEach(b => { b.UpdateRatios(); });
        }


        public static LatLngAlt SimToWorld(Vector3 itemPosition)
        {
            var beacon = m_BeaconList.First();
            return beacon.LatLngAlt(itemPosition);
        }

        public static Vector3 WorldToSim(LatLngAlt itemPosition)
        {
            var beacon = m_BeaconList.First();
            return beacon.XYZ(itemPosition);
        }


        [Header("Beacon Coordinates")]
        //[Range(-180.0f, 180.0f)]                                                                    
        public float latitude;

        //[Range(-90.0f, 90.0f)]
        public float longitude;
        
        //[Range(-1000.0f, 1000.0f)]
        public float altitude;


        void OnEnable(){
            RegisterBeacon(this);        
        }

        void OnDisable() { 
            UnregisterBeacon(this);
        }

        

        private float x_lng_ratio = 1;
        private float z_lat_ratio = 1;
        private float y_alt_ratio = 1;

        private void UpdateRatios()
        {
            var cnt = m_BeaconList.Count-1;
            if (cnt <= 0) { return; }
            float x_lng_ratio_tmp = 0;
            float y_alt_ratio_tmp = 0;
            float z_lat_ratio_tmp = 0;

            m_BeaconList.ForEach((beacon) => {
                if (beacon == this) return;
                var dpos = transform.position - beacon.transform.position;

                var dlng = (float)Mathf.Abs(beacon.longitude - longitude);
                var dlat = (float)Mathf.Abs(beacon.latitude - latitude);
                var dalt = (float)Mathf.Abs(beacon.altitude - altitude);

                x_lng_ratio_tmp += (dlng / dpos.x) / cnt;
                y_alt_ratio_tmp += (dalt / dpos.y) / cnt;
                z_lat_ratio_tmp += (dlat / dpos.z) / cnt;
            });

            x_lng_ratio = x_lng_ratio_tmp;
            y_alt_ratio = y_alt_ratio_tmp;
            z_lat_ratio = z_lat_ratio_tmp;
        }


        public LatLng GetLatLng(GameObject item)
        {
            return LatLng(item.transform.position);
        }

        public LatLng LatLng(Vector3 itemPosition)
        {
            var dpos = itemPosition - transform.position;
            var lng = dpos.x * x_lng_ratio + longitude;
            var lat = dpos.z * z_lat_ratio + latitude;
            
            return new LatLng(lat, lng);
        }

        public LatLngAlt LatLngAlt(Vector3 itemPosition)
        {
            var dpos = itemPosition - transform.position;
            var lng = itemPosition.x * x_lng_ratio + longitude;
            var lat = itemPosition.z * z_lat_ratio + latitude;
            var alt = itemPosition.y * y_alt_ratio + altitude;

            return new LatLngAlt(lat, lng, alt);
        }

        public Vector3 XYZ(LatLngAlt itemPosition)
        {
            var dpos = new Vector3();
            dpos.x = ((itemPosition.longitude - longitude) / x_lng_ratio);
            dpos.y = ((itemPosition.altitude - altitude) / y_alt_ratio);
            dpos.z = ((itemPosition.latitude - latitude) / z_lat_ratio);

            return dpos + transform.position;
        }





        public float DistanceTo(GPSBeacon other)
        {
            return DistanceTo(other.transform.position);
        }
        public float DistanceTo(Vector3 other) {
            var pos = transform.position;
            return Vector3.Distance(other, pos);
        }
    }
    public class LatLng
    {
        public float longitude, latitude;
        public LatLng(float longitude, float latitude) { 
            this.longitude = longitude;
            this.latitude = latitude;   
        }
    }
    public class LatLngAlt:LatLng
    {
        public float altitude;
        public LatLngAlt(float longitude, float latitude, float altitude):base(latitude,longitude)
        {
            this.altitude = altitude;
        }
    }



}