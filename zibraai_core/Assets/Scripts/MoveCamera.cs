using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public KeyCode forwardKey = KeyCode.UpArrow;
    public KeyCode backwardKey = KeyCode.DownArrow;

    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;

    public KeyCode upKey = KeyCode.PageUp;
    public KeyCode downKey = KeyCode.PageDown;

    public float speed = 0.1f;
    // Update is called once per frame
    void Update()
    {


        if (Input.GetKey(forwardKey))
            transform.Translate(Vector3.forward * speed, Space.Self);
        if (Input.GetKey(backwardKey))
            transform.Translate(Vector3.back * speed, Space.Self);


        if (Input.GetKey(leftKey))
            transform.Rotate(Vector3.down, Space.Self);

        if (Input.GetKey(rightKey))
            transform.Rotate(Vector3.up, Space.Self);



        if (Input.GetKey(upKey))
            transform.Translate(Vector3.up * speed, Space.Self);
        if (Input.GetKey(downKey))
            transform.Translate(Vector3.down * speed, Space.Self);
    }
}
