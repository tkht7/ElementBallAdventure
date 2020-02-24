using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementController : MonoBehaviour
{
    private GameObject container;
    private GameObject content1;
    private GameObject content2;
    private float count;
    
    void Start()
    {
        container = transform.Find("ElementContainer").gameObject;
        content1 = transform.Find("ElementContent1").gameObject;
        content2 = transform.Find("ElementContent2").gameObject;
        count = 0.0f;
    }
    
    void Update()
    {
        // アイテム取得後，一定時間経過でアイテム復活
        if (!container.activeSelf)
        {
            count += Time.deltaTime;
            if (count >= 5.0f)
            {
                container.SetActive(true);
                content1.SetActive(true);
                content2.SetActive(true);
                count = 0.0f;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            container.SetActive(false);
            content1.SetActive(false);
            content2.SetActive(false);
        }
    }
}
