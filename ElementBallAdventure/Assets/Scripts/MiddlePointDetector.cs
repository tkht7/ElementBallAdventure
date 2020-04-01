using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePointDetector : MonoBehaviour
{
    private AudioSource audioSource;
    public bool middlePointFlag;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        middlePointFlag = false;
    }
    
    void OnTriggerEnter(Collider collider)
    {
        // 中間ポイント到達判定(初回到達)
        if (collider.gameObject.CompareTag("Player") && !middlePointFlag)
        {
            middlePointFlag = true;
            audioSource.Play();
        }
    }
}
