using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalDetector : MonoBehaviour
{
    private Image goalMessage;
    private AudioSource audioSource;

    private Director director;
    public bool goalFlag;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        goalMessage = GameObject.Find("GoalMessage").GetComponent<Image>();
        director = GameObject.Find("Director").GetComponent<Director>();

        goalMessage.enabled = false;
        goalFlag = false;
    }

    //ゴールした時の処理
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && !goalFlag)
        {
            goalMessage.enabled = true; // ゴールメッセージを表示
            goalFlag = true;
            audioSource.Play();

            // ステージセレクトから始めた場合はタイトルに戻る
            string stageTransition;
            if (director.stageSelectFlag) stageTransition = "ReturnTitle";
            else                          stageTransition = "NextStage";
            // 3秒後にステージ遷移する
            Invoke(stageTransition, 3);
        }            
    }

    void NextStage()
    {
        director.NextStage();
    }

    void ReturnTitle()
    {
        director.ReturnTitle();
    }
}
