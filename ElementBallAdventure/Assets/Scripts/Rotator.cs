using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
        // アイテムを回転させる
        transform.Rotate(new Vector3(0.0f, 1500.0f * Time.deltaTime, 0.0f) * Time.deltaTime);
    }
}
