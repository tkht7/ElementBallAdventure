using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePointDetector : MonoBehaviour
{
    private Director director;
    private AudioSource audioSource;
    public bool middlePointFlag;

    void Start()
    {
        director = GameObject.Find("Director").GetComponent<Director>();
        audioSource = GetComponent<AudioSource>();
        // 中間ポイントから再開した時，すでに中間ポイントに到達していることにするため，directorから状態を取得
        middlePointFlag = director.middleResumeFlag;
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
