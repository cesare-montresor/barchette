using System;
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

    private float targetPosition = 0;
    private Vector3 startPosition;
    private Quaternion startRotation;

    // Update is called once per frame
    void Start()
    {
        

        HingeJoint joint = GetComponent<HingeJoint>();
        Rigidbody parent_rb = joint.connectedBody.GetComponent<Rigidbody>();

        
        startPosition = parent_rb.position;
        startRotation = parent_rb.rotation;


        targetPosition = joint.spring.targetPosition;
        joint.useSpring = true;
        joint.useLimits = true;
        joint.useMotor = false;

    }


    void Update()
    {
        HingeJoint joint = GetComponent<HingeJoint>();
        Rigidbody rb = GetComponent<Rigidbody>();

        Rigidbody parent_rb = joint.connectedBody.GetComponent<Rigidbody>();

        var spring = joint.spring;
        if (Input.GetKey(forwardKey) || Input.GetKey(backwardKey)) {
            parent_rb.isKinematic = false;
            var isForward = Input.GetKey(forwardKey);
            var addForce = isForward ? speed : -speed;
            var forceVector = Quaternion.Euler(0, (isForward ? 1:-1) * spring.targetPosition, 0) * Vector3.up * addForce;
            rb.AddRelativeForce(forceVector);
        }

        if (Input.GetKey(leftKey) || Input.GetKey(rightKey))
        {
            var addTorque = Input.GetKey(rightKey) ? torque : -torque;
            spring.targetPosition = Math.Max(Math.Min(spring.targetPosition + addTorque, joint.limits.max), joint.limits.min);
        }
        else {
            spring.targetPosition = ((spring.targetPosition - targetPosition) /2) * 0.01f;
        }
        joint.spring = spring;


        if (Input.GetKey(resetKet)) {
            parent_rb.isKinematic = true;
            var startTransform = GetComponentInParent<Transform>();
            parent_rb.position = startPosition;
            parent_rb.rotation = startRotation;
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
