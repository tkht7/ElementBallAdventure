using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloorController : MonoBehaviour
{
    public bool[] moveDirect = new bool[3];
    public bool[] reverse = new bool[3];
    public float moveDistance;
    public float moveSpeed;

    private Rigidbody rb;
    private float[] rev = { 1.0f, 1.0f, 1.0f };
    private float[] startPos = new float[3];
    private float[] updatePos = new float[3];

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos[0] = transform.position.x;
        startPos[1] = transform.position.y;
        startPos[2] = transform.position.z;
        for (int i = 0; i < reverse.Length; i++)
        {
            if (reverse[i])
                rev[i] = -1.0f;
            updatePos[i] = startPos[i];
        }
}

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < moveDirect.Length; i++)
        {
            if (moveDirect[i])
                updatePos[i] = startPos[i] + rev[i] * Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        }
        rb.MovePosition(new Vector3(updatePos[0], updatePos[1], updatePos[2]));
    }
}
