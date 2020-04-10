using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneGenerator : MonoBehaviour
{
    public GameObject WindZone;
    
    private AudioSource audioSource;

    private GameObject player;
    private const float shootRange = 60.0f;
    private float delta;
    private const float span = 4.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        delta = 0.0f;
    }
    
    void Update()
    {
        // プレイヤーが近づいた時だけ生成する
        if (player.transform.position.z >= transform.position.z - shootRange && player.transform.position.z <= transform.position.z)
        {
            delta += Time.deltaTime;
            if (delta > span)
            {
                delta = 0.0f;
                GameObject wind = Instantiate(WindZone) as GameObject;
                wind.transform.position = transform.position;
                audioSource.Play();
            }
        }
    }
}
