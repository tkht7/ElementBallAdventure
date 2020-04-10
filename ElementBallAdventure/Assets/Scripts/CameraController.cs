using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private GameObject mainCamera;
    private Quaternion startAngle;
    // AorDのみ押した時の視点移動の速さ
    private const float cameraRotationSpeed = 60.0f;
    
    void Start()
    {
        mainCamera = transform.Find("Main Camera").gameObject;
        transform.position = player.transform.position;
        startAngle = mainCamera.transform.rotation;
    }
    
    void Update()
    {
        // カメラ位置を45度おきの一番近いところに揃える
        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector3 angle = transform.localEulerAngles;
            float angleY = (float)Math.Floor((angle.y + 22.5f) % 360.0f / 45.0f) * 45.0f;
            transform.rotation = Quaternion.Euler(new Vector3(angle.x, angleY, angle.z));
        }

        // カメラを45度おきに回転させる
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(0.0f, -45.0f, 0.0f);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(0.0f, 45.0f, 0.0f);
            }
        }
        // カメラを回転させる
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0.0f, -cameraRotationSpeed * Time.deltaTime, 0.0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0.0f, cameraRotationSpeed * Time.deltaTime, 0.0f);
            }
        }
        transform.position = player.transform.position;
    }
}
