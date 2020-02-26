using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedWallController : MonoBehaviour
{
    private GameObject wall;
    private bool attackedFlag;
    private bool rotatedFlag;
    private float totalRotation;
    
    void Start()
    {
        wall = transform.Find("AttackedWall").gameObject;
        attackedFlag = false;
        rotatedFlag = false;
        totalRotation = 0.0f;
    }
    
    void Update()
    {
        // 相手が突進状態でぶつかってきたら壁を倒し始める
        if(attackedFlag && !rotatedFlag)
        {
            //transform.Rotate(60.0f * Time.deltaTime, 0.0f, 0.0f);
            totalRotation += 60.0f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(totalRotation, 0.0f, 0.0f);
            // 90度回転で止める(判定を少し早めにする)
            if (totalRotation >= 90.0f)
            {
                rotatedFlag = true;
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
        }
        // Debug.Log(transform.eulerAngles.x);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.gameObject.GetComponent<PlayerController>().rushFlag)
            {
                // 倒れたら反発係数をゼロにしてGroundタグをつける(ジャンプできるようにする)
                wall.GetComponent<BoxCollider>().material.bounciness = 0;
                transform.tag = "Ground";
                attackedFlag = true;
            }
        }
    }
}
