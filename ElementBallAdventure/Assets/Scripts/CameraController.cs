using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player; // 球のオブジェクト

    private GameObject mainCamera;
    private GameObject subCamera;

    private Vector3 offset; // 球からカメラまでの距離
    private Quaternion startAngle;
    
    
    void Start()
    {
        mainCamera = transform.Find("Main Camera").gameObject;
        subCamera = transform.Find("Sub Camera").gameObject;
        transform.position = player.transform.position;
        offset = mainCamera.transform.position - transform.position;
        startAngle = mainCamera.transform.rotation;
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0.0f, -1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.H))
        {
             transform.Rotate(0.0f, 1.0f, 0.0f);
        }
        transform.position = player.transform.position;

        if (Input.GetKeyDown(KeyCode.N))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            mainCamera.transform.rotation = startAngle;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            mainCamera.SetActive(!mainCamera.activeSelf);
            subCamera.SetActive(!subCamera.activeSelf);
        }
    }
}
