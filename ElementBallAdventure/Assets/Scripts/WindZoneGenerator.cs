using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneGenerator : MonoBehaviour
{
    public GameObject WindZone;
    
    public AudioClip windGenerateSound;
    private AudioSource audioSource;

    private GameObject player;
    private float span;
    private float delta;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        span = 4.0f;
        delta = 0.0f;
    }
    
    void Update()
    {
        // プレイヤーが近づいた時だけ生成する
        if (player.transform.position.z >= 236.0f && player.transform.position.z <= 296.0f)
        {
            delta += Time.deltaTime;
            if (delta > span)
            {
                delta = 0.0f;
                GameObject wind = Instantiate(WindZone) as GameObject;
                wind.transform.position = new Vector3(-7.0f, 32.0f, 306.0f);
                audioSource.PlayOneShot(windGenerateSound);
            }
        }
    }
}
