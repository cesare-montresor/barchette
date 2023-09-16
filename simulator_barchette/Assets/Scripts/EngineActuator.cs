using com.zibra.liquid.Manipulators;
using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Threading.Tasks;

public class EngineActuator : MonoBehaviour // ActuatorComponent // TODO: look into actuators as base class ?
{
	// public KeyCode forwardKey = KeyCode.W;
	// public KeyCode leftKey = KeyCode.A;
	// public KeyCode backwardKey = KeyCode.S;
	// public KeyCode rightKey = KeyCode.D;
	// public KeyCode resetKet = KeyCode.Q;

	public float speed = 5f;
	public float torque = 0.3f;
	public int ParticlesFullspeed = 50;

	private float targetPosition = 0;
	private Vector3 startPosition;
	private Quaternion startRotation;

	private Renderer render;
	private HingeJoint joint;
	private Rigidbody parent_rb;
	private bool isReady = true;
	


	// Update is called once per frame
	void Start()
	{
		render = GetComponent<Renderer>();

		joint = GetComponent<HingeJoint>();
		parent_rb = joint.connectedBody.GetComponent<Rigidbody>();

		startPosition = parent_rb.position;
		startRotation = parent_rb.rotation;
		
		targetPosition = joint.spring.targetPosition;
		joint.useSpring = true;
		joint.useLimits = true;
		joint.useMotor = false;

	}

	public float GetHingeAngle() {
		return joint.angle;
	}

	public float GetImmersion() {
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
		return immersion;
	}
	void UpdateEngineColor()
	{
		var immersion = GetImmersion();
		if (immersion > 0.8)
		{
			render.material.color = Color.green;
		}
		else if (immersion > 0.2)
		{
			render.material.color = Color.yellow;
		}
		else
		{
			render.material.color = Color.red;
		}
	}

	public void Forward() { Move(false); }
	public void Backward() { Move(true); }

	public void Move(bool reverse = false)
	{
		if (!isReady) { return; }
		Rigidbody rb = GetComponent<Rigidbody>();
		parent_rb.isKinematic = false;

		var spring = joint.spring;
		var isBackward = reverse;
		var addForce = isBackward ? -speed : speed;
		addForce *= GetImmersion();  // 0.0 <-> 1.0
		var forceVector = Quaternion.Euler(0, (isBackward ? -1 : 1) * spring.targetPosition, 0) * Vector3.up * addForce;
		rb.AddRelativeForce(forceVector);
	}

	public void TurnLeft()
	{
		if (!isReady) { return; }
		parent_rb.isKinematic = false;
		var spring = joint.spring;
		spring.targetPosition = Math.Max(Math.Min(spring.targetPosition + torque, joint.limits.max), joint.limits.min);
		joint.spring = spring;
	}

	public void TurnRight()
	{
		if (!isReady) { return; }
		parent_rb.isKinematic = false;
		var spring = joint.spring;
		spring.targetPosition = Math.Max(Math.Min(spring.targetPosition - torque, joint.limits.max), joint.limits.min);
		joint.spring = spring;
	}

	public void TurnNeutral()
	{
		var spring = joint.spring;
		//TODO: make it better
		spring.targetPosition = ((spring.targetPosition - targetPosition) / 2) * 0.01f;
		joint.spring = spring;
	}

	public void Restore()
	{
		isReady = false;

		parent_rb.isKinematic = true;
		parent_rb.position = startPosition;
		parent_rb.rotation = startRotation;
		
		var spring = joint.spring;
		spring.targetPosition = 0;
		joint.spring = spring;

		Task.Delay(1000).ContinueWith( t => isReady = true);
	}


	void Update() {
		UpdateEngineColor();
		// if (!isReady) { return; }
		// var spring = joint.spring;
		// if (Input.GetKey(forwardKey) || Input.GetKey(backwardKey)) {
		//     var isBackward = Input.GetKey(backwardKey);
		//     Move(isBackward);
		// }
		// if (Input.GetKey(leftKey))
		// {
		//     TurnLeft();
		// }
		// else if (Input.GetKey(rightKey)) {
		//     TurnRight();
		// }
		// else {
		//     TurnNeutral();
		// }
		// if (Input.GetKey(resetKet))
		// {
		//     Restore();
		// }
		// if (Input.GetKey(KeyCode.A))
		//     rb.AddForce(Vector3.left);
		// if (Input.GetKey(KeyCode.D))
		//     rb.AddForce(Vector3.right);
		// if (Input.GetKey(KeyCode.W))
		//     rb.AddForce(Vector3.up);
		// if (Input.GetKey(KeyCode.S))
		//     rb.AddForce(Vector3.down);
	}

}
