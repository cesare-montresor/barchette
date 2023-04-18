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

        var parts = GetComponentsInChildren<Rigidbody>();
        m_boat = parts.Where(part => part.name=="boat").FirstOrDefault();

        var ray_sensors = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
        m_sonar = ray_sensors.Where(ray_sensor => ray_sensor.name == "sonar").FirstOrDefault();
        m_lidar = ray_sensors.Where(ray_sensor => ray_sensor.name == "lidar").FirstOrDefault();
     
        //m_imu = GameObject.Find("imu").GetComponent<RigidBodySensorComponent>();
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
        var action = actions.DiscreteActions.ToArray();
        if (action[1] != 0)
        {
            m_engine.Move();
        }
        else if (action[2] != 0)
        {
            m_engine.Move(false);
        }
        else if (action[3] != 0)
        {
            m_engine.TurnLeft();
        }
        else if (action[4] != 0)
        {
            m_engine.TurnRight();
        }

    }
    
    /*
     * Add Manual Control 
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    */

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        m_engine.Restore();
    }
}
