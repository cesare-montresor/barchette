using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using com.zibra.liquid.Manipulators;
using System.Linq;
using Univr.Barchette.Sensors.GPS;
using Univr.Barchette.Sensors.IMU;
using Univr.Barchette.Sensors.Engine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    public float engineSpeed = 30.0f;
    public float engineTorque = 1.0f;

    [Header("Manual controls")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;

    public KeyCode resetKet = KeyCode.Q;

    [Header("Relevant Game Objects")]
    public Transform boatObject;
    public Transform targetObject;
    [Header("Scene Limits")]
    public Transform sceneLimitA;
    public Transform sceneLimitB;
    
    private Vector3 m_sceneLimitMin = new Vector3( -10000, -10000, -10000);
    private Vector3 m_sceneLimitMax = new Vector3(  10000,  10000,  10000);



    Rigidbody m_boat;
    RayPerceptionSensorComponent3D m_sonar;
    RayPerceptionSensorComponent3D m_lidar;
    IMUSensorComponent m_imu;
    GPSSensorComponent m_gps;
    EngineSensorComponent[] m_engine_sensors;
    DifferentialEngine m_engine;

    public override void Initialize() {

        var parts = GetComponentsInChildren<Rigidbody>();
        m_boat = parts.Where(part => part.name=="boat").FirstOrDefault();

        var ray_sensors = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
        m_sonar = ray_sensors.Where(ray_sensor => ray_sensor.name == "sonar").FirstOrDefault();
        m_lidar = ray_sensors.Where(ray_sensor => ray_sensor.name == "lidar").FirstOrDefault();

        m_imu = GetComponentsInChildren<IMUSensorComponent>().FirstOrDefault();
        m_gps = GetComponentsInChildren<GPSSensorComponent>().FirstOrDefault(); 

        m_engine = GetComponentsInChildren<DifferentialEngine>().FirstOrDefault();
        m_engine_sensors = GetComponentsInChildren<EngineSensorComponent>();


        if (m_lidar != null) { 
            m_lidar.enabled = useLidar; 
        }
        if (m_sonar != null) { m_sonar.enabled = useSonar; }
        if (m_imu != null) { m_imu.enabled = useIMU; }
        if (m_gps != null) {
            m_gps.useLatLng = useGPS;
            m_gps.useAltitude = useAltitude;
            m_gps.useCompass = useCompass;
        }


        foreach (var engine_sensor in m_engine_sensors) { 
            engine_sensor.enabled = useEngineSensors; 
        }

        if (m_engine != null) {
            m_engine.Speed = engineSpeed;
            m_engine.Torque = engineTorque;
        }

        if (sceneLimitA != null && sceneLimitB != null)
        {
            m_sceneLimitMin.x = Mathf.Min(sceneLimitA.position.x, sceneLimitB.position.x);
            m_sceneLimitMin.y = Mathf.Min(sceneLimitA.position.y, sceneLimitB.position.y);
            m_sceneLimitMin.z = Mathf.Min(sceneLimitA.position.z, sceneLimitB.position.z);
            m_sceneLimitMax.x = Mathf.Max(sceneLimitA.position.x, sceneLimitB.position.x);
            m_sceneLimitMax.y = Mathf.Max(sceneLimitA.position.y, sceneLimitB.position.y);
            m_sceneLimitMax.z = Mathf.Max(sceneLimitA.position.z, sceneLimitB.position.z);
        }

    }
	
	// Listener for the action received, both from the neural network and the keyboard
	// (if heuristic mode), inside the Python script, the action is passed with the step funciton
    public override void OnActionReceived(ActionBuffers actions) {
        var action = actions.DiscreteActions;
        //if (action[0] == 0) { /*NOOP*/ }
        
        if (action[0] == 0) { m_engine.Forward(); }
        if (action[0] == 1) { m_engine.TurnLeft(); }
        if (action[0] == 2) { m_engine.TurnRight(); }


        // Checking if the boat is inside the environment
        bool breakLimits = (
            (boatObject.position.x < m_sceneLimitMin.x || boatObject.position.x > m_sceneLimitMax.x) ||
            (boatObject.position.y < m_sceneLimitMin.y || boatObject.position.y > m_sceneLimitMax.y) ||
            (boatObject.position.z < m_sceneLimitMin.z || boatObject.position.z > m_sceneLimitMax.z)
        );

		// Special reward flag if the boat is outside the environment
        if (breakLimits)
        {
            SetReward(-1);
            //EndEpisode();
        }
        else {
            SetReward(0);
        }
    }


	// Listener for the observations collections.ww
	public override void CollectObservations(VectorSensor sensor) {
		float distance = Vector3.Distance( boatObject.transform.position, targetObject.transform.position );

        Debug.Log(distance);
        Debug.Log(boatObject.position);
        Debug.Log(targetObject.position);
        
        sensor.AddObservation( distance );
	}
    

	// Debug function, useful to control the agent with the keyboard in heurisitc mode
    public override void Heuristic(in ActionBuffers actionsOut) {
        var action = actionsOut.DiscreteActions;

        action[0] = 0;
        if      (Input.GetKey(forwardKey))  { action[0] = 0; }
        else if (Input.GetKey(leftKey))     { action[0] = 1; }
        else if (Input.GetKey(rightKey))    { action[0] = 2; }
    }
    

	// Called at the every new episode of the training loop,
	// after each reset (both from target, crash or timeout)
    public override void OnEpisodeBegin() {
        m_engine.Restore();
    }

}
