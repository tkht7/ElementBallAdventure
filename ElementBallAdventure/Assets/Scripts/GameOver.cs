using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private Director director;
    private GameObject player;
    private bool calledFlag = false;
    private const float gameOverHeight = -80.0f;

    void Start()
    {
        director = GameObject.Find("Director").GetComponent<Director>();
        player = GameObject.Find("Player").gameObject;
    }
    
    void Update()
    {
        if(player.transform.position.y <= gameOverHeight && !calledFlag)
        {
            calledFlag = true;
            director.measureTimeFlag = false;
            director.GameOver();
        }
    }
}
