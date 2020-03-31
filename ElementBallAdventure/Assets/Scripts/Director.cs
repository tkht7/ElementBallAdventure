using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Director : SingletonMonoBehaviour<Director>
{
    [System.NonSerialized]
    public int currentStageNum = 0; //現在のステージ番号（0はStartScene）

    [SerializeField]
    private string[] stageName; //ステージ名
    [SerializeField]
    private GameObject fadeCanvasPrefab;
    [SerializeField]
    private GameObject gameOverCanvasPrefab;
    [SerializeField]
    private float fadeWaitTime = 1.0f; //フェード時の待ち時間

    public AudioClip titleSound;
    public AudioClip decisionSound;
    public AudioClip gameOverSound;
    public AudioClip[] gameClearSounds;
    private AudioSource audioSource;

    private GameObject fadeCanvasClone;
    private FadeCanvas fadeCanvas;
    private GameObject gameOverCanvasClone;
    private GameObject middlePoint;
    private GameObject player;
    private Rigidbody playerRigidbody;
    private Button[] buttons;
    private EventTrigger[] eventTriggers;

    private bool middleResumeFlag;

    public void Awake()
    {
        // 既にインスタンスが存在したら削除
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        //シーンを切り替えてもこのゲームオブジェクトを削除しないようにする
        DontDestroyOnLoad(gameObject);
        //デリゲートの登録
        SceneManager.sceneLoaded += OnSceneLoaded;

        middlePoint = GameObject.Find("MiddlePoint");
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(titleSound); // スタート画面での音
        middleResumeFlag = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //改めて取得
        middlePoint = GameObject.Find("MiddlePoint");
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }


    void Update()
    {
        // スタート画面から最初のステージへ
        if (currentStageNum == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.PlayOneShot(decisionSound);
            currentStageNum++;
            MoveToStage(currentStageNum);
        }

        // クリア画面からスタート画面へ
        if (currentStageNum == stageName.Length - 1 && Input.GetKeyDown(KeyCode.Space))
        {
            currentStageNum = 0;
            MoveToStage(currentStageNum);
        }
    }

    //次のステージに進む処理
    public void NextStage()
    {
        middleResumeFlag = false;
        currentStageNum++;
        MoveToStage(currentStageNum);
    }

    //任意のステージに移動する処理
    public void MoveToStage(int stageNum)
    {
        //コルーチンを実行
        StartCoroutine(WaitForLoadScene(stageNum));
    }


    //シーンの読み込みと待機を行うコルーチン
    IEnumerator WaitForLoadScene(int stageNum)
    {
        // キャラの移動を停止させる
        playerRigidbody.isKinematic = true;
        //フェードオブジェクトを生成
        fadeCanvasClone = Instantiate(fadeCanvasPrefab);
        //コンポーネントを取得
        fadeCanvas = fadeCanvasClone.GetComponent<FadeCanvas>();
        //フェードインさせる
        fadeCanvas.fadeIn = true;
        yield return new WaitForSeconds(fadeWaitTime);
        //シーンを非同期で読込し、読み込まれるまで待機する
        yield return SceneManager.LoadSceneAsync(stageName[stageNum]);
        Debug.Log(player.transform.position);
        // 中間ポイントが存在するかどうか
        if (middlePoint != null)
        {
            // 中間ポイントを取っていたら，そこから再開
            if (middleResumeFlag)
            {
                player.transform.position = middlePoint.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
                player.transform.rotation = middlePoint.transform.rotation;
            }
        }
        //フェードアウトさせる
        fadeCanvas.fadeOut = true;
        // スタート画面での音
        if (stageNum == 0)
        {
            audioSource.PlayOneShot(titleSound);
        }
        // クリア画面での音
        else if (stageNum == stageName.Length - 1)
        {
            for(int i = 0; i < gameClearSounds.Length; i++)
            {
                audioSource.PlayOneShot(gameClearSounds[i]);
            }
        }
    }

    //ゲームオーバー処理
    public void GameOver()
    {
        // キャラの移動を停止させる
        playerRigidbody.isKinematic = true;
        
        //ゲームオーバー画面表示
        gameOverCanvasClone = Instantiate(gameOverCanvasPrefab);

        // 効果音
        audioSource.PlayOneShot(gameOverSound);

        //ボタンを取得
        buttons = gameOverCanvasClone.GetComponentsInChildren<Button>();
        buttons[0].Select();

        // イベントトリガーを取得
        eventTriggers = gameOverCanvasClone.GetComponentsInChildren<EventTrigger>();

        //マウスとキーボード選択を両立
        eventTriggers[0].triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry entry1;

        entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerExit;
        entry1.callback.AddListener((eventData) => SelectSelf(buttons[0]));
        eventTriggers[0].triggers.Add(entry1);

        entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((eventData) => NonSelectSelf());
        eventTriggers[0].triggers.Add(entry1);


        // 二つ目のボタンについても同様
        eventTriggers[1].triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry entry2;

        entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener((eventData) => SelectSelf(buttons[1]));
        eventTriggers[1].triggers.Add(entry2);

        entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerEnter;
        entry2.callback.AddListener((eventData) => NonSelectSelf());
        eventTriggers[1].triggers.Add(entry2);

        // ボタンにイベント設定
        buttons[0].onClick.AddListener(Retry);
        buttons[1].onClick.AddListener(Return);
    }

    public void SelectSelf(Button button)
    {
        button.Select();
    }

    public void NonSelectSelf()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    // リトライ
    public void Retry()
    {
        // 中間ポイントに達しているか確認
        if (player.GetComponent<PlayerController>().middlePointFlag)
            middleResumeFlag = true;
        Destroy(gameOverCanvasClone);
        MoveToStage(currentStageNum);
    }

    // スタート画面に戻る
    public void Return()
    {
        Destroy(gameOverCanvasClone);
        currentStageNum = 0;
        MoveToStage(currentStageNum);
    }

}
