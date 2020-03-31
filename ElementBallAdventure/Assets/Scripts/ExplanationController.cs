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
        if (collider.gameObject.CompareTag("Player"))
        {
            explanation.SetActive(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            explanation.SetActive(false);
        }
    }
}
