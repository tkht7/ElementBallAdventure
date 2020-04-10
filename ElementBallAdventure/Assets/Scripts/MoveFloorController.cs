using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloorController : MonoBehaviour
{
    // 動く方向(どの軸に沿って動くか)
    [SerializeField]
    private bool[] moveDirect = new bool[3];
    // 最初に動く向きを反転
    [SerializeField]
    private bool[] reverse = new bool[3];
    // 動く距離
    [SerializeField]
    private float moveDistance;
    // 動く速さ
    [SerializeField]
    private float moveSpeed;

    private Rigidbody rb;
    private float[] rev = { 1.0f, 1.0f, 1.0f };
    private float[] startPos = new float[3];
    private float[] updatePos = new float[3];
    
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
