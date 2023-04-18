using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Univr.Barchette.Sensors.GPS
{
    
    public class GPSSensorComponent : SensorComponent
    {
        public override ISensor[] CreateSensors()
        {
            var sensor = new GPSSensor("gps", this);
            return new ISensor[] { sensor };
        }

        public string sensorName = "GPSSensor";
        public float errorMeters = 4.0f;
    }



}