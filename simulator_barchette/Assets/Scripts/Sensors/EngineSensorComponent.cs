using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Univr.Barchette.Sensors.Engine
{
    /// <summary>
    /// IMUSensorComponent SensorComponent, MonoBehaviour  that generates observations form a fake GPS.
    /// </summary>
    public class EngineSensorComponent : SensorComponent
    {
        [Header("Engine")]
        public string sensorName = "EngineSensor";
        
        public override ISensor[] CreateSensors()
        {
            var sensor = new EngineSensor(sensorName, this);
            return new ISensor[] { sensor };
        }
    }


    /// <summary>
    /// ISensor implementation that generates observations form a fake GPS.
    /// </summary>
    public class EngineSensor : ISensor
    {
        private static object m_SensorCountLock = new object();
        private int m_SensorCount = 0;
        private string m_Basename = "EngineSensor";
        private string m_Name;
        private int m_SensorID;
        private EngineSensorComponent m_Parent;
        private EngineActuator m_Engine;


        private float m_lastUpdate;
        private float m_lastImmersion;

        public EngineSensor(string name, EngineSensorComponent parent)
        {
            lock (m_SensorCountLock)
            {
                m_SensorCount++;
                m_SensorID = m_SensorCount;
            }
            m_Basename += $":{m_SensorID}";
            m_Name = parent.sensorName;
            m_Parent = parent;
            m_Engine = m_Parent.GetComponent<EngineActuator>();

            Reset();
        }


        
        public virtual byte[] GetCompressedObservation()
        {
            return null;
        }

        
        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public string GetName()
        {
            return m_Basename + ":" + m_Name;
        }

        public int ObservationSize() {
            if (!m_Parent.enabled) { return 0;  }
            return 1;
        }

        public ObservationSpec GetObservationSpec()
        {
            int size = ObservationSize();
            return ObservationSpec.Vector(size);
        }

        public void Reset()
        {
            m_lastImmersion = 0;
            m_lastUpdate = Time.fixedTime;
        }
    
        public void Update()
        {
            if (m_Engine == null) {
                Reset();
                return;
            }
            m_lastImmersion = m_Engine.GetImmersion();
            m_lastUpdate = Time.fixedTime;
        }

        public int Write(ObservationWriter writer)
        {
            var obs = new List<float> { m_lastImmersion };
            writer.AddList(obs);
            return ObservationSize();
        }
    }


}