using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // 球のオブジェクト

    private Vector3 offset; // 球からカメラまでの距離
    
    void Start()
    {
        offset = transform.position - player.transform.position;
    }
    
    void Update()
    {
        transform.position = player.transform.position + offset;
    }
}
