using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneController : MonoBehaviour
{
    private Vector3 startPos;
    // -z方向以外にも打ち出されるときは変更できるようにする
    private float windSpeed;
    private float windPersistence;
    private float windPower;

    public AudioClip windZoneSound;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        startPos = transform.position;
        windSpeed = -40.0f;
        windPersistence = 86.0f;
        windPower = 6000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, 0.0f, windSpeed * Time.deltaTime);
        if (startPos.z - transform.position.z > windPersistence)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            var rb = collider.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.back * windPower * Time.deltaTime);
            audioSource.PlayOneShot(windZoneSound);
        }
    }
}
