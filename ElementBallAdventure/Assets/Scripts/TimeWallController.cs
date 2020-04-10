using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWallController : MonoBehaviour
{
    private AudioSource audioSource;
    private float pushCount; // ボタンの状態の判定に余裕を持たせるためのカウント（効果音用）

    private GameObject pivot;       // 壁の基準位置
    private GameObject button;      // 押すと壁が動くボタン
    private bool pushFlag;          // ボタンを押しているフラグ
    private Vector3 startPivotPos;  // 壁の基準位置の初期位置
    private Vector3 startButtonPos; // ボタンの初期位置
    private const float pivotPosLimit = 8.0f;  // 壁の動く位置の限界
    private const float buttonPosLimit = 0.2f; // ボタンの押し込まれる位置の限界
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pushCount = 0.0f;
        pivot = transform.Find("TimeWallPivot").gameObject;
        button = transform.Find("TimeWallButton").gameObject;
        pushFlag = false;
        startPivotPos = pivot.transform.position;
        startButtonPos = button.transform.position;
    }
    
    void Update()
    {
        if (pushFlag) // ボタンが押されてるときは壁を上げて，ボタンを下げる(押されてる感じにする)
        {
            // ボタンの押し込みの制限
            if (startButtonPos.y - button.transform.position.y < buttonPosLimit)
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y - Time.deltaTime, tempPos.z);
                // 制限を超えた分戻す
                if (startButtonPos.y - button.transform.position.y > buttonPosLimit)
                    button.transform.position = new Vector3(tempPos.x, startButtonPos.y - buttonPosLimit, tempPos.z);
            }
            // 壁を上げる位置の制限
            if (pivot.transform.position.y - startPivotPos.y < pivotPosLimit)
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y + Time.deltaTime * pivotPosLimit, tempPos.z);
                // 壁が上がってる間だけ効果音をならす
                if (pushCount <= 0.0f)
                    audioSource.Play();
                pushCount = 0.3f;
                // 制限を超えた分戻す
                if (pivot.transform.position.y - startPivotPos.y > pivotPosLimit)
                {
                    pivot.transform.position = new Vector3(tempPos.x, startPivotPos.y + pivotPosLimit, tempPos.z);
                    audioSource.Stop(); // 壁が上がり終えたら効果音を止める
                }
                
            }
        }
        else
        {
            // 元の位置まで戻るようにする
            if (startButtonPos.y - button.transform.position.y > 0.0f)
            {
                var tempPos = button.transform.position;
                button.transform.position = new Vector3(tempPos.x, tempPos.y + Time.deltaTime, tempPos.z);
                // 元の位置以上に上がらないようにする
                if (startButtonPos.y - button.transform.position.y < 0.0f)
                    button.transform.position = startButtonPos;
            }
            // 元の位置まで下げる
            if (pivot.transform.position.y - startPivotPos.y > 0.0f)
            {
                var tempPos = pivot.transform.position;
                pivot.transform.position = new Vector3(tempPos.x, tempPos.y - Time.deltaTime, tempPos.z);
                // 元の位置以上に下がらないようにする
                if (pivot.transform.position.y - startPivotPos.y < 0.0f)
                    pivot.transform.position = startPivotPos;
            }
            // ボタンを押していない時は効果音を止める（押していない判定には余裕を持たせる）
            if (pushCount <= 0.0f)
                audioSource.Stop();
            pushCount -= Time.deltaTime;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // 壁につぶされて地面をすり抜けるのを防ぐ
            if(Mathf.Abs(collision.gameObject.transform.position.x - transform.position.x) < 0.6f && // 壁の真下付近にプレイヤーがいたとき　&&
               pivot.transform.position.y - startPivotPos.y < 1.0f)                                  // 壁が閉まりかけの時
            {
                float dir;
                // プレイヤーのいる位置からどっちに押し出すか決める
                if (collision.gameObject.transform.position.x - transform.position.x < 0)
                    dir = -1.0f;
                else
                    dir = 1.0f;
                var tempPos = collision.gameObject.transform.position;
                // 壁のない位置にプレイヤーを押し出す
                collision.gameObject.transform.position = new Vector3(pivot.transform.position.x+dir, tempPos.y, tempPos.z);

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
