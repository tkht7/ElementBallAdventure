using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloorController : MonoBehaviour
{
    public int moveDirect; // 0: X方向, 1: Y方向, 2: Z方向
    public float moveDistance;
    public float moveSpeed;
    public bool reverse;

    private Rigidbody rb;
    private float rev;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        rev = 1.0f;
        if (reverse)
            rev = -1.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (moveDirect == 0)
            rb.MovePosition(new Vector3(startPos.x + rev * Mathf.PingPong(Time.time * moveSpeed, moveDistance), startPos.y, startPos.z));
        else if (moveDirect == 1)
            rb.MovePosition(new Vector3(startPos.x, startPos.y + rev * Mathf.PingPong(Time.time * moveSpeed, moveDistance), startPos.z));
        else if (moveDirect == 2)
            rb.MovePosition(new Vector3(startPos.x, startPos.y, startPos.z + rev * Mathf.PingPong(Time.time * moveSpeed, moveDistance)));

    }
}
