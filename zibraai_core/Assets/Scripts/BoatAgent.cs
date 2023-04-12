using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using UnityEngine;
using Unity.MLAgents.Extensions.Sensors;
using Unity.VisualScripting;
using com.zibra.liquid.Manipulators;
using System.Linq;
using System;

public class BoatAgent : Agent
{
    public bool useVecObs;
    EnvironmentParameters m_ResetParams;

    Rigidbody m_boat;
    RayPerceptionSensorComponent3D m_sonar, m_lidar;
    RigidBodySensorComponent m_imu;

    MoveEngine m_engine;
    HingeJoint m_engineHinge;
    ZibraLiquidDetector m_engineSensor;

    public override void Initialize()
    {
        base.Initialize();

        m_boat = GameObject.Find("boat").GetComponent<Rigidbody>();
        m_sonar = GameObject.Find("sonar").GetComponent<RayPerceptionSensorComponent3D>();
        m_lidar = GameObject.Find("lidar").GetComponent<RayPerceptionSensorComponent3D>();
        m_imu = GameObject.Find("imu").GetComponent<RigidBodySensorComponent>();
        m_engine = GameObject.Find("engine").GetComponent<MoveEngine>();
        m_engineHinge = GameObject.Find("engine").GetComponent<HingeJoint>();
        m_engineSensor = GameObject.Find("engine").GetComponentInChildren<ZibraLiquidDetector>();
        
        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        
        //IMU
        //m_imu.

        //Lidar
        var rays = m_lidar.RaySensor.RayPerceptionOutput.RayOutputs;
        foreach (var ray in rays)
        {
            var dist = Vector3.Distance(ray.StartPositionWorld, ray.EndPositionWorld);
            sensor.AddObservation(dist);
        }

        //Sonar
        rays = m_sonar.RaySensor.RayPerceptionOutput.RayOutputs;
        foreach (var ray in rays)
        {
            var dist = Vector3.Distance(ray.StartPositionWorld, ray.EndPositionWorld);
            sensor.AddObservation(dist);
        }


    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }
}
