using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationPanelController : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody rb;
    // 加算する速度の基準
    private const float accel = 1.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // 乗るとプレイヤーを加速させるパネルの挙動
            var tempV = rb.velocity; // プレイヤーの現在の速度を取得
            var accelX = -accel * Mathf.Sin(transform.localEulerAngles.y * Mathf.PI / 180.0f);
            var accelZ = -accel * Mathf.Cos(transform.localEulerAngles.y * Mathf.PI / 180.0f);
            rb.velocity = new Vector3(tempV.x + accelX, tempV.y, tempV.z + accelZ); // パネルの向きに応じて速度を加算
        }
    }
}
