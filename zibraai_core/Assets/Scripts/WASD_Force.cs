using UnityEngine;

public class WASD_Force : MonoBehaviour
{
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    public float speed = 1f;
    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (Input.GetKey(leftKey))
            rb.AddRelativeForce(Vector3.left);
        if (Input.GetKey(rightKey))
            rb.AddRelativeForce(Vector3.right);

        if (Input.GetKey(forwardKey))
            rb.AddRelativeForce(Vector3.back * speed);
        if (Input.GetKey(backwardKey))
            rb.AddRelativeForce(Vector3.forward * speed);

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
