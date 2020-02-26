using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationPanelController : MonoBehaviour
{
    public AudioClip accelSound;
    private AudioSource audioSource;

    private float accel;
    private Rigidbody rb;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        accel = 1.0f;
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            var tempV = rb.velocity;
            rb.velocity = new Vector3(tempV.x, tempV.y, tempV.z + accel); // +Z以外に進むやつがいる時はrotationから向きを求めて作る

            audioSource.PlayOneShot(accelSound);
        }
    }
}
