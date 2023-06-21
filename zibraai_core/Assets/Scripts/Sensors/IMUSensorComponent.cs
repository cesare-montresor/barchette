using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Univr.Barchette.Sensors.IMU
{
    /// <summary>
    /// IMUSensorComponent SensorComponent, MonoBehaviour  that generates observations form a fake GPS.
    /// </summary>
    public class IMUSensorComponent : SensorComponent
    {
        [Header("Sensor")]
        public string sensorName = "IMUSensor";

        [Header("Gyroscopes")]
        public bool useAccelerometer = true;
        [Range(0,1)]
        public float errorRateGyroscope = 0.05f;

        [Header("Accelerometers")]
        public bool useGyroscope = true;
        [Range(0, 1)]
        public float errorRateAccelerometer = 0.05f;
        public float scaleForces = 1.0f;
        public float minimumDeltaTime = 0.00001f;
        public override ISensor[] CreateSensors()
        {
            var sensor = new IMUSensor("imu", this);
            return new ISensor[] { sensor };
        }
    }


    /// <summary>
    /// ISensor implementation that generates observations form a fake GPS.
    /// </summary>
    public class IMUSensor : ISensor
    {
        private static object m_SensorCountLock = new object();
        private int m_SensorCount = 0;
        private string m_Basename = "IMUSensor";
        private string m_Name;
        private int m_SensorID;
        private IMUSensorComponent m_parent;


        private float m_lastUpdate;
        private Vector3 m_lastRotation;
        private Vector3 m_lastPosition;
        private Vector3 m_lastSpeed;
        private Vector3 m_lastAccelleration;



        public IMUSensor(string name, IMUSensorComponent parent)
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

        public int ObservationSize() {
            if (!m_parent.enabled) { return 0; }

            int size = 0;
            if (m_parent.useGyroscope) size += 3;
            if (m_parent.useAccelerometer) size += 3;
            return size;
        }

        public ObservationSpec GetObservationSpec()
        {
            int size = ObservationSize();
            return ObservationSpec.Vector(size);
        }

        public void Reset()
        {
            m_lastPosition = m_parent.transform.position;
            m_lastUpdate = Time.fixedTime;
        }

        private Vector3 getErrorVector(float rate) {
            System.Random random = new System.Random();
            Vector3 errors = new Vector3();
            errors.x = (float)((random.NextDouble() * rate * 2) - rate) + 1;
            errors.y = (float)((random.NextDouble() * rate * 2) - rate) + 1;
            errors.z = (float)((random.NextDouble() * rate * 2) - rate) + 1;
            return errors;
        }

        public void Update()
        {
            m_lastRotation = m_parent.transform.rotation.eulerAngles;
            //Gyro
            if (m_parent.errorRateGyroscope > 0)
            {
                var errors = getErrorVector(m_parent.errorRateGyroscope);
                m_lastRotation.x *= errors.x;
                m_lastRotation.y *= errors.y;
                m_lastRotation.z *= errors.z;
            }


            //Accell
            var deltaTime = Time.fixedTime - m_lastUpdate;
            if (deltaTime < m_parent.minimumDeltaTime) { return; } //skip time increments too small.
            var deltaPos = m_parent.transform.position - m_lastPosition;
            var speed = deltaPos / deltaTime;
            var deltaSpeed = speed - m_lastSpeed;

            if (m_parent.errorRateAccelerometer > 0) {
                var errors = getErrorVector(m_parent.errorRateAccelerometer);
                deltaSpeed.x *= errors.x;
                deltaSpeed.y *= errors.y;
                deltaSpeed.z *= errors.z;
            }

            m_lastAccelleration = deltaSpeed;
            m_lastSpeed = speed;
            m_lastPosition = m_parent.transform.position;
            m_lastUpdate = Time.fixedTime;
        }                               

        public int Write(ObservationWriter writer)
        {
            if (m_parent.useGyroscope) {
                writer.Add(m_lastRotation);
            }
            if (m_parent.useAccelerometer){
                writer.Add(m_lastAccelleration);
            }
            return ObservationSize();
        }
    }


}