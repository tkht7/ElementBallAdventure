using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementController : MonoBehaviour
{
    private GameObject container;
    private GameObject content1;
    private GameObject content2;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        container = transform.Find("ElementContainer").gameObject;
        content1 = transform.Find("ElementContent1").gameObject;
        content2 = transform.Find("ElementContent2").gameObject;
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!container.activeSelf)
        {
            count++;
            if (count >= 300)
            {
                container.SetActive(true);
                content1.SetActive(true);
                content2.SetActive(true);
                count = 0;
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
