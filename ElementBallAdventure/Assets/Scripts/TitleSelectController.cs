using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSelectController : MonoBehaviour
{
    private Director director;

    private GameObject selectMenu;
    private Button fromtheBeginningButton;
    private const int NewGame = 0;

    private GameObject stageSelectMenu;
    private Button stage1Button;

    private int screenState;
    private const int title = 0;
    private const int menu = 1;
    private const int stageMenu = 2;

    private AudioSource audioSource;
    public AudioClip decisionSound;

    void Start()
    {
        director = GameObject.Find("Director").GetComponent<Director>();
        selectMenu = transform.Find("SelectMenu").gameObject;
        stageSelectMenu = transform.Find("StageSelectMenu").gameObject;
        fromtheBeginningButton = transform.Find("SelectMenu/FromtheBeginningButton").GetComponent<Button>();
        stage1Button = transform.Find("StageSelectMenu/Stage1Button").GetComponent<Button>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && screenState == title)
        {
            audioSource.PlayOneShot(decisionSound);
            selectMenu.SetActive(true);
            screenState = menu;
            fromtheBeginningButton.Select();
        }
        else if (Input.GetKeyDown(KeyCode.X) && screenState == menu)
        {
            selectMenu.SetActive(false);
            screenState = title;
        }
        else if (Input.GetKeyDown(KeyCode.X) && screenState == stageMenu)
        {
            stageSelectMenu.SetActive(false);
            selectMenu.SetActive(true);
            screenState = menu;
            fromtheBeginningButton.Select();
        }
    }

    // はじめからorステージセレクト
    public void StartSelect(int select)
    {
        if (select == NewGame)
        {
            audioSource.PlayOneShot(decisionSound);
            director.currentStageNum++;
            director.MoveToStage(director.currentStageNum);
        }
        else
        {
            audioSource.PlayOneShot(decisionSound);
            stageSelectMenu.SetActive(true);
            selectMenu.SetActive(false);
            screenState = stageMenu;
            stage1Button.Select();
        }
    }

    // StageSelect後のボタン押下時
    public void StageSelect(int stage)
    {
        audioSource.PlayOneShot(decisionSound);
        director.currentStageNum = stage;
        director.stageSelectFlag = true;
        director.MoveToStage(director.currentStageNum);
    }
}
