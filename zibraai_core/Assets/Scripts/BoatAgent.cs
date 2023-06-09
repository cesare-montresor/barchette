using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using com.zibra.liquid.Manipulators;
using System.Linq;
using Univr.Barchette.Sensors.GPS;
using Univr.Barchette.Sensors.IMU;
using Univr.Barchette.Sensors.Engine;

public class BoatAgent : Agent
{
    [Header("Sensors")]
    public bool useLidar = false;
    public bool useSonar = true;
    public bool useIMU = true;
    public bool useGPS = true;
    public bool useAltitude = true;
    public bool useCompass = true;
    public bool useCamera = false;
    public bool useEngineSensors = true;

    [Header("Engine")]
    public float engineSpeed = 10.0f;
    public float engineTorque = 10.0f;

    [Header("Manual controls")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;

    public KeyCode resetKet = KeyCode.Q;


    Rigidbody m_boat;
    RayPerceptionSensorComponent3D m_sonar;
    RayPerceptionSensorComponent3D m_lidar;
    IMUSensorComponent m_imu;
    GPSSensorComponent m_gps;
    EngineSensorComponent m_engine_sensor;

    EngineActuator m_engine;
    

    public override void Initialize()
    {
        //base.Initialize();
        
        var parts = GetComponentsInChildren<Rigidbody>();
        m_boat = parts.Where(part => part.name=="boat").FirstOrDefault();

        var ray_sensors = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
        m_sonar = ray_sensors.Where(ray_sensor => ray_sensor.name == "sonar").FirstOrDefault();
        m_lidar = ray_sensors.Where(ray_sensor => ray_sensor.name == "lidar").FirstOrDefault();

        m_imu = GetComponentsInChildren<IMUSensorComponent>().FirstOrDefault();
        m_gps = GetComponentsInChildren<GPSSensorComponent>().FirstOrDefault(); 

        m_engine = GetComponentsInChildren<EngineActuator>().FirstOrDefault();
        m_engine_sensor = GetComponentsInChildren<EngineSensorComponent>().FirstOrDefault();


        m_lidar.enabled = useLidar;
        m_sonar.enabled = useSonar;
        m_imu.enabled = useIMU;
        m_gps.useLatLng = useGPS;
        m_gps.useAltitude = useAltitude;
        m_gps.useCompass = useCompass;
        m_engine_sensor.enabled = useEngineSensors;

        m_engine.speed = engineSpeed;
        m_engine.torque = engineTorque;
        

    }
    /*
    public override void CollectObservations(VectorSensor sensor)
    { }
      */
    public override void OnActionReceived(ActionBuffers actions)
    {
        // base.OnActionReceived(actions);
        var action = actions.DiscreteActions;
        if (action[0] == 1) { m_engine.Forward(); }
        if (action[0] == 2) { m_engine.Backward(); }
        if (action[0] == 3) { m_engine.TurnLeft(); }
        if (action[0] == 4) { m_engine.TurnRight(); }
    }
    
    
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;

        action[0] = 0;
        if      (Input.GetKey(forwardKey))  { action[0] = 1; }
        else if (Input.GetKey(backwardKey)) { action[0] = 2; }
        else if (Input.GetKey(leftKey))     { action[0] = 3; }
        else if (Input.GetKey(rightKey))    { action[0] = 4; }
    }
    

    public override void OnEpisodeBegin()
    {
        m_engine.Restore();
    }
}
