using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedWallController : MonoBehaviour
{
    private GameObject pivot;
    private GameObject tp;
    private bool attackedFlag;
    private bool rotatedFlag;

    // Start is called before the first frame update
    void Start()
    {
        pivot = transform.root.gameObject;
        tp = GameObject.Find("TargetPaint");
        attackedFlag = false;
        rotatedFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackedFlag && !rotatedFlag)
        {
            pivot.transform.Rotate(1.0f, 0.0f, 0.0f);
            if (pivot.transform.eulerAngles.x == 90.0f)
            {
                rotatedFlag = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>().attackFlag)
            {
                //Debug.Log(GetComponent<BoxCollider>().material);
                GetComponent<BoxCollider>().material.bounciness = 0;
                transform.tag = "Ground";
                tp.transform.tag = "Ground";
                attackedFlag = true;
            }
        }
    }
}
