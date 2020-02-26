using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private Director director;
    private GameObject player;
    private bool calledFlag = false;

    void Start()
    {
        director = GameObject.Find("Director").GetComponent<Director>();
        player = GameObject.Find("Player").gameObject;
    }
    
    void Update()
    {
        if(player.transform.position.y <= -100.0f && !calledFlag)
        {
            calledFlag = true;
            director.GameOver();
        }
    }
}
