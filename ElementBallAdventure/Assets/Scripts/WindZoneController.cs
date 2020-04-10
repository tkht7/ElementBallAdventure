using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneController : MonoBehaviour
{
    private Vector3 startPos;
    private const float windSpeed = -40.0f;
    private const float windPersistence = 86.0f;
    private const float windPower = 6000.0f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startPos = transform.position;
    }
    
    void Update()
    {
        // 風のかたまりを一定のスピードで進ませる
        transform.Translate(0.0f, 0.0f, windSpeed * Time.deltaTime);
        // 一定距離進んだら消える
        if (startPos.z - transform.position.z > windPersistence)
        {
            Destroy(gameObject);
        }
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
        // プレイヤーに触れたら，プレイヤーに力を加える
        if (collider.gameObject.CompareTag("Player"))
        {
            var rb = collider.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.back * windPower * Time.deltaTime);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            audioSource.Stop();
        }
    }
}
