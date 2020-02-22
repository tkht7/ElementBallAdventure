using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedWallController : MonoBehaviour
{
    private GameObject wall;
    private GameObject tp;
    private bool attackedFlag;
    private bool rotatedFlag;

    // Start is called before the first frame update
    void Start()
    {
        wall = transform.Find("AttackedWall").gameObject;
        tp = transform.Find("TargetPaint").gameObject;
        attackedFlag = false;
        rotatedFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackedFlag && !rotatedFlag)
        {
            transform.Rotate(1.0f, 0.0f, 0.0f);
            if (transform.eulerAngles.x >= 88.5f)
            {
                rotatedFlag = true;
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
        }
        // Debug.Log(transform.eulerAngles.x);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (collider.gameObject.GetComponent<PlayerController>().attackFlag)
            {
                wall.GetComponent<BoxCollider>().material.bounciness = 0;
                transform.tag = "Ground";
                attackedFlag = true;
            }
        }
    }
}
