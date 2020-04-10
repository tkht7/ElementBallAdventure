using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationController : MonoBehaviour
{
    private GameObject explanation;

    void Start()
    {
        explanation = transform.Find("Explanation").gameObject;
    }

    void OnTriggerEnter(Collider collider)
    {
        // プレイヤーが看板から一定の範囲内に入ったら説明を表示する
        if (collider.gameObject.CompareTag("Player"))
        {
            explanation.SetActive(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // 範囲外に行ったら表示を消す
        if (collider.gameObject.CompareTag("Player"))
        {
            explanation.SetActive(false);
        }
    }
}
