using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedWallController : MonoBehaviour
{
    private GameObject wall;
    private bool attackedFlag;
    private bool rotatedFlag;
    private float totalRotation;
    // 壁が倒れる速さ
    private const float rotationSpeed = 60.0f;
    
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
            totalRotation += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(totalRotation, 0.0f, 0.0f);
            // 90度回転で止める
            if (totalRotation >= 90.0f)
            {
                rotatedFlag = true;
                transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            }
        }
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
