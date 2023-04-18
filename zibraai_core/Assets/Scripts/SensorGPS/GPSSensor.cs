using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Univr.Barchette.Sensors.GPS
{
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

        private GPSTerrain gpsTerrain;
        private float m_lat;
        private float m_lng;
        private float m_alt;
        private GPSSensorComponent m_parent;

        public GPSSensor(string name, GPSSensorComponent parent) {
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
            return m_Basename +":"+ m_Name;
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(3);
        }

        public void Reset()
        {
            var pos = m_parent.transform.position;

            Scene scene = SceneManager.GetActiveScene();
            var terrainSet = new HashSet<GPSTerrain>();
            scene.GetRootGameObjects().ToList().ForEach(gobj => { terrainSet.Concat(gobj.GetComponents<GPSTerrain>()); terrainSet.Concat(gobj.GetComponentsInChildren<GPSTerrain>()); });
            List<GPSTerrain> terrains = terrainSet.ToList();
            terrains.Sort((GPSTerrain t1, GPSTerrain t2) => (int)(t1.DistanceTo(pos) - t2.DistanceTo(pos)));
            gpsTerrain = terrains.FirstOrDefault();
        }

        public void Update()
        {
            if (gpsTerrain == null){ return; }
            var pos = m_parent.transform.position;
            var posT = gpsTerrain.terrain.transform.position;
            var sizeT = gpsTerrain.terrain.transform.lossyScale/2;
            var deltas = (pos - posT);
            var relX = deltas[0] / sizeT[0];
            var relY = deltas[1] / sizeT[1];
            var relZ = deltas[2] / sizeT[2];
            m_lng = relX * (gpsTerrain.longitudeMax - gpsTerrain.longitudeMin);
            m_lat = relY * (gpsTerrain.latitudeMax - gpsTerrain.latitudeMin);
            m_alt = relZ * (gpsTerrain.altitudeMax - gpsTerrain.altitudeMin);


            // TDOO: do it better then this: https://gis.stackexchange.com/a/385618
            // https://stackoverflow.com/questions/7222382/get-lat-long-given-current-point-distance-and-bearing/51765950#51765950
            var lng_error = m_parent.errorMeters / (111111 * Mathf.Cos(Mathf.Deg2Rad*m_lat));
            var lat_error = m_parent.errorMeters / 111111;
            var alt_error = m_parent.errorMeters;
            
            m_lng += lng_error * ( (Random.value * 2) - 1);
            m_lat += lat_error * ((Random.value * 2) - 1);
            m_alt += alt_error * ((Random.value * 2) - 1); 
        }

        public int Write(ObservationWriter writer)
        {
            var data = new List<float> { m_lng, m_lat, m_alt };
            writer.AddList(data);
            return data.Count();
        }
    }

}