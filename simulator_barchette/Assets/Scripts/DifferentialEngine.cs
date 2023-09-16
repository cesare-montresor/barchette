using com.zibra.liquid.Manipulators;
using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Threading.Tasks;

public class DifferentialEngine : MonoBehaviour // ActuatorComponent // TODO: look into actuators as base class ?
{
    // public KeyCode forwardKey = KeyCode.W;
    // public KeyCode leftKey = KeyCode.A;
    // public KeyCode backwardKey = KeyCode.S;
    // public KeyCode rightKey = KeyCode.D;
    // public KeyCode resetKet = KeyCode.Q;

    public float Speed = 20f;
    public float Torque = 2.0f;
    public float ReverseRatio = -0.5f;

    public int ParticlesFullspeed = 10;

	private float targetPosition = 0;
	private Vector3 startPosition;
	private Quaternion startRotation;

	private Renderer render;
    private FixedJoint leftEngineJoint;
    private FixedJoint rightEngineJoint;
    private ZibraLiquidDetector leftEngineSensor;
    private ZibraLiquidDetector rightEngineSensor;
    private Renderer leftEngineRender;
    private Renderer rightEngineRender;
    private Rigidbody parent_rb;
	private bool isReady = true;
	
                                                                         

	// Update is called once per frame
	void Start()
	{
		render = GetComponent<Renderer>();

        var lds = GetComponentsInChildren<ZibraLiquidDetector>();
        foreach (var ld in lds)
        {
            if (ld.name == "left_sensor")
            {
                leftEngineSensor = ld;
            }
            else if (ld.name == "right_sensor")
            {
                rightEngineSensor = ld;
            }
        }


        var engineJoints = GetComponentsInChildren<FixedJoint>();
        foreach (var joint in engineJoints)
        {
            var t = joint.transform;
            if (t.name == "left_engine")
            {
                parent_rb = joint.connectedBody;
                leftEngineRender = t.GetComponent<Renderer>();
                leftEngineJoint = joint;
            }
            else if (t.name == "right_engine")
            {
                parent_rb = joint.connectedBody;
                rightEngineRender = t.GetComponent<Renderer>();
                rightEngineJoint = joint;
            }
        }

		startPosition = parent_rb.position;
		startRotation = parent_rb.rotation;
	}

    public float GetImmersionLeft()
    {
        if (leftEngineSensor == null) return 1.0f;
        return GetImmersion(leftEngineSensor);
    }

    public float GetImmersionRight()
    {
        if (rightEngineSensor == null) return 1.0f;
        return GetImmersion(rightEngineSensor);
    }

    public float GetImmersion(ZibraLiquidDetector ld) {
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
        var immersionLeft = GetImmersionLeft();
        if (immersionLeft > 0.8){
            leftEngineRender.material.color = Color.green;
		}else if (immersionLeft > 0.2){
            leftEngineRender.material.color = Color.yellow;
		}else{
            leftEngineRender.material.color = Color.red;
		}

        var immersionRight = GetImmersionRight();
        if (immersionRight > 0.8){
            rightEngineRender.material.color = Color.green;
		}else if (immersionRight > 0.2){
            rightEngineRender.material.color = Color.yellow;
		}else{
            rightEngineRender.material.color = Color.red;
		}
	}

	public void Forward() { Move(true, true); }
	public void Backward() { Move(false, false); }

    public void TurnLeft() { Move(false, true); }
    public void TurnRight() { Move(true, false); }
    public void Move(bool forwardLeft, bool forwardRight)
	{
		if (!isReady) { return; }
		 
		parent_rb.isKinematic = false;
        var forceVectorLeft = Vector3.up * Speed * GetImmersionLeft() * (forwardLeft ? 1 : -1); 
        var forceVectorRight = Vector3.up * Speed * GetImmersionRight() * (forwardRight ? 1 : -1);
        if (forwardLeft == forwardRight)
        {
            forceVectorLeft *= 0.5f;
            forceVectorRight *= 0.5f;
        }
        else
        {
            forceVectorLeft *= Torque * (forwardLeft? 1: -ReverseRatio);
            forceVectorRight *= Torque * (forwardRight ? 1 : -ReverseRatio); ;
        }                                               
        
        leftEngineJoint.GetComponent<Rigidbody>().AddRelativeForce(forceVectorLeft);
        rightEngineJoint.GetComponent<Rigidbody>().AddRelativeForce(forceVectorRight);

	}
   
	public void Restore()
	{
		isReady = false;

		parent_rb.isKinematic = true;
		parent_rb.position = startPosition;
		parent_rb.rotation = startRotation;
		
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
