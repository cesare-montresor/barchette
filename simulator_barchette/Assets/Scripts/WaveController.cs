using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public KeyCode enableLoop = KeyCode.L;
        
    public Vector3 offset = new Vector3(0,0,1) * 5;
    public float timePush = 0.1f;
    public float timePushed = 0.1f;
    public float timePull = 0.8f;
    public float timePulled = 1.0f;

    private double startTime = -1;
    private DateTime epochStart;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var timeTotal = timePush + timePushed + timePull + timePulled;

        if (Input.GetKey(enableLoop))
        {
            if (startTime == -1)
            {
                startTime = (DateTime.UtcNow - epochStart).TotalSeconds;
            }
            else {
                startTime = -1;
            }
        }

        if (startTime != -1)
        {
            var dt = ((DateTime.UtcNow - epochStart).TotalSeconds - startTime) % timeTotal;
            if (dt <= timePush)
            {
                transform.position = startPosition + ((float)(dt / timePush) * offset);
            }
            var dtPull = dt - timePush - timePushed;
            if (dtPull > 0 && dtPull <= timePull)
            {
                transform.position = (startPosition+ offset) - ((float)(dtPull / timePull) * offset);
            }
        }




    }
}
