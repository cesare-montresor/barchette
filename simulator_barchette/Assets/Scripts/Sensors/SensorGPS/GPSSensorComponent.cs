using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Univr.Barchette.Sensors.GPS
{
    /// <summary>
    /// GPSSensorComponent SensorComponent, MonoBehaviour  that generates observations form a fake GPS.
    /// </summary>
    public class GPSSensorComponent : SensorComponent
    {
        public override ISensor[] CreateSensors()
        {
            var sensor = new GPSSensor("gps", this);
            return new ISensor[] { sensor };
        }

        public string sensorName = "GPSSensor";
        public float errorMeters = 4.0f;

        public bool useLatLng = true;
        public bool useAltitude = true;
        public bool useCompass = true;

        public float compassOffset = 0.0f;
    }


    /// <summary>
    /// ISensor implementation that generates observations form a fake GPS.
    /// </summary>
    public class GPSSensor : ISensor
    {
        private static object m_SensorCountLock = new object();
        private int m_SensorCount = 0;
        private string m_Basename = "GPSSensor";
        private string m_Name;
        private int m_SensorID;



        private float m_lat;
        private float m_lng;
        private float m_alt;
        private float m_comp;
        private GPSSensorComponent m_parent;
        //private GPSBeacon m_beacon;

        public GPSSensor(string name, GPSSensorComponent parent)
        {
            lock (m_SensorCountLock)
            {
                m_SensorCount++;
                m_SensorID = m_SensorCount;
            }
            m_Basename += $":{m_SensorID}";
            m_Name = parent.sensorName;
            m_parent = parent;

            Reset();
        }


        /// <inheritdoc/>
        public virtual byte[] GetCompressedObservation()
        {
            return null;
        }

        /// <inheritdoc/>
        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public string GetName()
        {
            return m_Basename + ":" + m_Name;
        }

        public ObservationSpec GetObservationSpec()
        {
            int size = 0;
            
            if (m_parent.useLatLng) size += 2;
            if (m_parent.useAltitude) size += 1;
            if (m_parent.useCompass) size += 1;
            if (!m_parent.enabled) { size = 0; }

            return ObservationSpec.Vector(size);
        }

        public void Reset()
        {

            /*
            Scene scene = SceneManager.GetActiveScene();
            var gpsBeacons = new HashSet<GPSBeacon>();
            scene.GetRootGameObjects().ToList().ForEach(gobj => { gpsBeacons.Concat(gobj.GetComponents<GPSBeacon>()); gpsBeacons.Concat(gobj.GetComponentsInChildren<GPSBeacon>()); });
            List<GPSBeacon> beacons = gpsBeacons.ToList();
            beacons.Sort((GPSBeacon t1, GPSBeacon t2) => (int)(t1.DistanceTo(pos) - t2.DistanceTo(pos)));
            m_beacon = beacons.FirstOrDefault();
            */
        }

        public void Update()
        {
            var pos = m_parent.transform.position;
            var coords = GPSBeacon.SimToWorld(pos);

            // TDOO: do it better then this: https://gis.stackexchange.com/a/385618
            // https://stackoverflow.com/questions/7222382/get-lat-long-given-current-point-distance-and-bearing/51765950#51765950
            var lng_error = m_parent.errorMeters / (111111 * Mathf.Cos((Mathf.Deg2Rad * m_lat)));
            var lat_error = m_parent.errorMeters / 111111;
            var alt_error = m_parent.errorMeters;

            m_lng = coords.longitude + lng_error * ((Random.value * 2) - 1);
            m_lat = coords.latitude + lat_error * ((Random.value * 2) - 1);
            m_alt = coords.altitude + alt_error * ((Random.value * 2) - 1);
            m_comp = Mathf.Abs(m_parent.transform.rotation.eulerAngles.y + m_parent.compassOffset) % 360;
        }

        public int Write(ObservationWriter writer)
        {
            var data = new List<float> { m_lng, m_lat };

            if (!m_parent.useLatLng) data.Clear();
            if (m_parent.useAltitude) data.Add(m_alt);
            if (m_parent.useCompass) data.Add(m_comp);

            writer.AddList(data);
            return data.Count();
        }
    }


}