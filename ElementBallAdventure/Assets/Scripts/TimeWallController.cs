using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWallController : MonoBehaviour
{
    private GameObject pivot; // 壁の基準位置
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
        if (pushFlag) // ボタンが押されてるときは壁を上げて，ボタンを下げる(押されてる感じにする)
        {
            if(startButtonPos.y - button.transform.position.y < 0.2f) // 押し込みの制限
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y - 0.01f, tempPos.z);
            }
            if(pivot.transform.position.y - startPivotPos.y < 8.0f) // 壁を上げる位置の制限
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y + 0.1f, tempPos.z);
            }
        }
        else
        {
            if (startButtonPos.y - button.transform.position.y > 0.0f) // 元の位置まで戻るようにする
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y + 0.01f, tempPos.z);
            }
            if (pivot.transform.position.y - startPivotPos.y > 0.0f) // 元の位置以上に下がらないようにする
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y - 0.02f, tempPos.z);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // 壁につぶされて地面をすり抜けるのを防ぐ
            if(Mathf.Abs(collision.gameObject.transform.position.x - transform.position.x) < 0.6f && // 壁の真下付近にプレイヤーがいたとき
               pivot.transform.position.y - startPivotPos.y < 1.0f) // 壁が閉まりかけの時
            {
                var dir = 0.0f;
                if (collision.gameObject.transform.position.x - transform.position.x < 0)
                    dir = -1.0f;
                else
                    dir = 1.0f;
                var tempPos = collision.gameObject.transform.position;
                collision.gameObject.transform.position = new Vector3(pivot.transform.position.x+dir, tempPos.y, tempPos.z); // 壁のない位置にプレイヤーを移動

                // 違う向きで配置するときはyについて同じことをする
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
