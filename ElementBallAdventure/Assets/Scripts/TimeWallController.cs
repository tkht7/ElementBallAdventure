using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWallController : MonoBehaviour
{
    public AudioClip timeWallSound;
    private AudioSource audioSource;

    private GameObject pivot; // 壁の基準位置
    private GameObject button;
    private bool pushFlag;
    private Vector3 startPivotPos;
    private Vector3 startButtonPos;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
                button.transform.position = new Vector3(tempPos.x, tempPos.y - Time.deltaTime, tempPos.z);
                if (startButtonPos.y - button.transform.position.y > 0.2f) // 制限を超えた分戻す
                    button.transform.position = new Vector3(tempPos.x, startButtonPos.y - 0.2f, tempPos.z);

                audioSource.PlayOneShot(timeWallSound);
            }
            if(pivot.transform.position.y - startPivotPos.y < 8.0f) // 壁を上げる位置の制限
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y + Time.deltaTime * 8.0f, tempPos.z);
                if (pivot.transform.position.y - startPivotPos.y > 8.0f) // 制限を超えた分戻す
                    pivot.transform.position = new Vector3(tempPos.x, startPivotPos.y + 8.0f, tempPos.z);
            }
        }
        else
        {
            if (startButtonPos.y - button.transform.position.y > 0.0f) // 元の位置まで戻るようにする
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y + Time.deltaTime, tempPos.z);
                if (startButtonPos.y - button.transform.position.y < 0.0f) // 元の位置以上に上がらないようにする
                    button.transform.position = startButtonPos;
            }
            if (pivot.transform.position.y - startPivotPos.y > 0.0f) // 元の位置まで下げる
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y - Time.deltaTime, tempPos.z);
                if (pivot.transform.position.y - startPivotPos.y < 0.0f) // 元の位置以上に下がらないようにする
                    pivot.transform.position = startPivotPos;
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
                if (collision.gameObject.transform.position.x - transform.position.x < 0) // プレイヤーのいる位置からどっちに押し出すか決める
                    dir = -1.0f;
                else
                    dir = 1.0f;
                var tempPos = collision.gameObject.transform.position;
                collision.gameObject.transform.position = new Vector3(pivot.transform.position.x+dir, tempPos.y, tempPos.z); // 壁のない位置にプレイヤーを押し出す

                // 違う向きで配置するときはyについて同じことをするようにする
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        // ボタンにプレイヤーが乗った時，ジャンプできるようにする
        if (collider.gameObject.CompareTag("Player"))
        {
            tag = "Ground";
        }
        pushFlag = true;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            tag = "Untagged";
        }
        pushFlag = false;
    }
}
