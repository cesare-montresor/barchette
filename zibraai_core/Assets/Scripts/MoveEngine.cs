using com.zibra.liquid.Manipulators;
using System;
using System.Net.NetworkInformation;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MoveEngine : MonoBehaviour
{
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;

    public KeyCode resetKet = KeyCode.Q;

    public float speed = 5f;
    public float torque = 0.3f;
    public int ParticlesFullspeed = 50;

    private float targetPosition = 0;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Renderer renderer;
    private HingeJoint joint;
    private Rigidbody parent_rb;

    // Update is called once per frame
    void Start()
    {
        renderer = GetComponent<Renderer>();

        joint = GetComponent<HingeJoint>();
        parent_rb = joint.connectedBody.GetComponent<Rigidbody>();


        startPosition = parent_rb.position;
        
        targetPosition = joint.spring.targetPosition;
        joint.useSpring = true;
        joint.useLimits = true;
        joint.useMotor = false;

    }

    float GetImmersion() {
        ZibraLiquidDetector ld = GetComponentInChildren<ZibraLiquidDetector>();
        float immersion;
        if (ParticlesFullspeed > 0)
        {
            immersion = (float)Math.Min(ld.ParticlesInside, ParticlesFullspeed) / ParticlesFullspeed;
        }
        else
        {
            immersion = 1.0f;
        }

        if (immersion > 0.8)
        {
            renderer.material.color = Color.green;
        }
        else if (immersion > 0.3)
        {
            renderer.material.color = Color.yellow;
        }
        else
        {
            renderer.material.color = Color.red;
        }
        return immersion;
    }

    void Push(bool reverse = false)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        parent_rb.isKinematic = false;

        var spring = joint.spring;
        var isBackward = reverse;
        var addForce = isBackward ? -speed : speed;
        addForce *= GetImmersion();  // 0.0 <-> 1.0
        var forceVector = Quaternion.Euler(0, (isBackward ? -1 : 1) * spring.targetPosition, 0) * Vector3.up * addForce;
        rb.AddRelativeForce(forceVector);
    }

    void TurnLeft()
    {
        var spring = joint.spring;
        var addTorque = Input.GetKey(leftKey) ? torque : -torque;
        spring.targetPosition = Math.Max(Math.Min(spring.targetPosition + torque, joint.limits.max), joint.limits.min);
        joint.spring = spring;

    }

    void TurnRight()
    {
        var spring = joint.spring;
        spring.targetPosition = Math.Max(Math.Min(spring.targetPosition - torque, joint.limits.max), joint.limits.min);
        joint.spring = spring;

    }

    void TurnNeutral()
    {
        var spring = joint.spring;
        spring.targetPosition = ((spring.targetPosition - targetPosition) / 2) * 0.01f;
        joint.spring = spring;
    }

    private void Restore()
    {
        parent_rb.isKinematic = true;
        var startTransform = GetComponentInParent<Transform>();
        parent_rb.position = startPosition;
        parent_rb.rotation = startRotation;
    }

    void Update()
    {
        
        
        var spring = joint.spring;
        if (Input.GetKey(forwardKey) || Input.GetKey(backwardKey)) {
            var isBackward = Input.GetKey(backwardKey);
            Push(isBackward);
        }

        if (Input.GetKey(leftKey))
        {
            TurnLeft();
        }
        else if (Input.GetKey(rightKey)) {
            TurnRight();
        }
        else {
            TurnNeutral();
        }



        if (Input.GetKey(resetKet)) {
            Restore();
        }


        //float xDir = Input.GetAxis("Horizontal");
        //float yDir = Input.GetAxis("Vertical");
        //Vector3 moveDir = new Vector3(xDir* speed, 0, yDir* speed);
        //rb.AddRelativeForce(moveDir);
        //rb.AddForce(moveDir);
        /*
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(Vector3.left);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(Vector3.right);
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(Vector3.up);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(Vector3.down);
        */
    }
}
