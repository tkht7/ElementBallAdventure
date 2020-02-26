using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalDetector : MonoBehaviour
{
    public Text goalText;
    private AudioSource audioSource;

    private Director director;
    private bool calledFlag = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        director = GameObject.Find("Director").GetComponent<Director>();
        goalText.text = "";
    }

    //ゴールした時の処理
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && !calledFlag)
        {
            calledFlag = true;
            goalText.text = "GOAL!";
            audioSource.Play();

            // 3秒後にステージ遷移する
            Invoke("NextStage", 3);
        }            
    }

    void NextStage()
    {
        director.NextStage();
    }
}
