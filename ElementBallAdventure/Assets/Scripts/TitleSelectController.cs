using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSelectController : MonoBehaviour
{
    private Director director;

    private GameObject selectMenu;
    private Button newGameButton;
    private const int NewGame = 0;

    private GameObject stageSelectMenu;
    private Button stage1Button;

    // タイトル画面の状態
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
        newGameButton = transform.Find("SelectMenu/NewGameButton").GetComponent<Button>();
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
            newGameButton.Select();
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
            newGameButton.Select();
        }
    }

    // はじめからorステージセレクト
    public void StartSelect(int select)
    {
        if (select == NewGame)
        {
            audioSource.PlayOneShot(decisionSound);
            director.currentStageNum++;
            director.stageSelectFlag = false;
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

    // ステージ選択
    public void StageSelect(int stage)
    {
        audioSource.PlayOneShot(decisionSound);
        director.currentStageNum = stage;
        director.stageSelectFlag = true;
        director.MoveToStage(director.currentStageNum);
    }
}
