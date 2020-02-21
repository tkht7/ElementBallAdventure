using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWallController : MonoBehaviour
{
    private GameObject pivot;
    private GameObject button;
    private bool pushFlag;
    Vector3 startPivotPos;
    Vector3 startButtonPos;

    // Start is called before the first frame update
    void Start()
    {
        pivot = transform.Find("TimeWallPivot").gameObject;
        button = transform.Find("TimeWallButton").gameObject;
        pushFlag = false;
        startPivotPos = pivot.transform.position;
        startButtonPos = button.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pushFlag)
        {
            if(startButtonPos.y - button.transform.position.y < 0.2f)
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y - 0.01f, tempPos.z);
            }
            if(pivot.transform.position.y - startPivotPos.y < 8.0f)
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y + 0.1f, tempPos.z);
            }
        }
        else
        {
            if (startButtonPos.y - button.transform.position.y > 0.0f)
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y + 0.01f, tempPos.z);
            }
            if (pivot.transform.position.y - startPivotPos.y > 0.0f)
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y - 0.02f, tempPos.z);
            }
        }
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pushFlag = true;
            tag = "Ground";
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pushFlag = false;
            tag = "Untagged";
        }
    }
}
