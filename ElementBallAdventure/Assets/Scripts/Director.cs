using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Director : SingletonMonoBehaviour<Director>
{
    [System.NonSerialized]
    public int currentStageNum = 0; //現在のステージ番号
    private const int startScene = 0;
    private const int stage1 = 1;
    private const int stage2 = 2;
    private const int stage3 = 3;
    private const int clearScene = 4;

    [SerializeField]
    private string[] stageName; //ステージ名
    [SerializeField]
    private GameObject fadeCanvasPrefab; // シーン遷移するときに挟む黒い画面
    [SerializeField]
    private GameObject gameOverCanvasPrefab; // ゲームオーバー時に表示する画面
    [SerializeField]
    private float fadeWaitTime = 1.0f; //フェード時の待ち時間

    // 効果音やBGMなど
    public AudioClip titleSound;
    public AudioClip decisionSound;
    public AudioClip gameOverSound;
    public AudioClip[] gameClearSounds;
    public AudioClip[] BGM;
    private AudioSource[] audioSource;

    private GameObject camera;
    private GameObject fadeCanvasClone;
    private FadeCanvas fadeCanvas;
    private GameObject gameOverCanvasClone;
    private GameObject middlePoint;
    private GameObject goal;
    private GoalDetector goalDetector;
    private GameObject player;
    private Rigidbody playerRigidbody;
    private Button[] buttons;
    private EventTrigger[] eventTriggers;
    private const int retry = 0;
    private const int title = 1;

    public bool middleResumeFlag; // 中間地点に達しているかどうか
    public bool stageSelectFlag; // はじめから遊んでいるか，ステージセレクトで遊んでいるか
    public bool transitionFlag; // シーン遷移を複数行わないように制御

    [System.NonSerialized]
    public List<float> clearTime = new List<float> { 0.0f, 0.0f, 0.0f };
    private Text clearTimeText;
    private Text stage1ResultText;
    private Text stage2ResultText;
    private Text stage3ResultText;
    private Text endResultText;
    public bool measureTimeFlag; // これがtrueの時のみ時間を計測する

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

        camera = GameObject.Find("CameraRelation");
        middlePoint = GameObject.Find("MiddlePoint");
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        clearTimeText = transform.Find("Canvas/Text").GetComponent<Text>();
        audioSource = GetComponents<AudioSource>();
        middleResumeFlag = false;
        transitionFlag = false;
        stageSelectFlag = false;
        measureTimeFlag = false;

        // タイトル画面での音を鳴らす
        audioSource[0].PlayOneShot(titleSound);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //改めて取得
        camera = GameObject.Find("CameraRelation");
        middlePoint = GameObject.Find("MiddlePoint");
        goal = GameObject.Find("Goal");
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        if (goal != null)
            goalDetector = goal.GetComponent<GoalDetector>();

        // 各シーンでの処理
        if (currentStageNum == startScene)
        {
            audioSource[0].PlayOneShot(titleSound);
            clearTime = new List<float> { 0.0f, 0.0f, 0.0f }; // 時間をリセット
        }
        else if (currentStageNum == clearScene)
        {
            stage1ResultText = GameObject.Find("Stage1ResultText").GetComponent<Text>();
            stage2ResultText = GameObject.Find("Stage2ResultText").GetComponent<Text>();
            stage3ResultText = GameObject.Find("Stage3ResultText").GetComponent<Text>();
            endResultText = GameObject.Find("EndResultText").GetComponent<Text>();

            // 各ステージのクリアタイムの合計を計算
            float totalTime = 0.0f;
            for (int i = 0; i < clearTime.Count; i++)
                totalTime += clearTime[i];
            // 各ステージと合計のタイムを表示
            stage1ResultText.text = "Stage1: " + timeShaping(clearTime[0]);
            stage2ResultText.text = "Stage2: " + timeShaping(clearTime[1]);
            stage3ResultText.text = "Stage3: " + timeShaping(clearTime[2]);
            endResultText.text = "TotalTime: " + timeShaping(totalTime);
            // クリア時の効果音を流す
            for (int i = 0; i < gameClearSounds.Length; i++)
                audioSource[0].PlayOneShot(gameClearSounds[i]);
        }
        else // startSceneとclearScene以外ではBGMを流す
        {
            audioSource[1].Play(); // BGM
            measureTimeFlag = true; //このフラグがある時のみタイムを計測する
        }
    }


    void Update()
    {
        // クリア画面からスタート画面へ
        if (currentStageNum == clearScene && Input.GetKeyDown(KeyCode.Z))
        {
            currentStageNum = 0;
            MoveToStage(currentStageNum);
        }

        if (goalDetector != null)
        {
            // ゴールしたらBGMを止め，タイムも止める
            if (goalDetector.goalFlag)
            {
                audioSource[1].Stop();
                measureTimeFlag = false;
            }
        }

        if (measureTimeFlag)
        {
            // 各ステージでのタイムを計測
            clearTime[currentStageNum - 1] += Time.deltaTime;
        }

        if (currentStageNum != startScene && currentStageNum != clearScene)
        {
            // 現在のタイムを表示形式に合わせて変換
            clearTimeText.text = "Time: " + timeShaping(clearTime[currentStageNum - 1]);
        }
        else
        {
            // スタート画面，クリア画面ではタイムは表示しない(左上に表示するタイムのこと)
            clearTimeText.text = "";
        }
    }

    // 計測タイムを00:00:00(分:秒:小数部)の形式に変換
    public string timeShaping(float time)
    {
        return ((int)time / 60).ToString("00") + ":"             // 分
               + ((int)time % 60).ToString("00") + ":"           // 秒
               + (time - (int)time).ToString("F2").Substring(2); // 小数部
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
        // 中間ポイントが存在するかどうか
        if (middlePoint != null)
        {
            // 中間ポイントを取っていたら，そこから再開
            if (middleResumeFlag)
            {
                player.transform.position = middlePoint.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
                camera.transform.rotation = middlePoint.transform.rotation;
            }
        }
        //フェードアウトさせる
        fadeCanvas.fadeOut = true;
        // シーン遷移が終わったことを示す
        transitionFlag = false;
    }

    //ゲームオーバー処理
    public void GameOver()
    {
        // キャラの移動を停止させる
        playerRigidbody.isKinematic = true;
        //ゲームオーバー画面表示
        gameOverCanvasClone = Instantiate(gameOverCanvasPrefab);
        // 効果音
        audioSource[0].PlayOneShot(gameOverSound);
        // BGM停止
        audioSource[1].Stop();
        // シーン遷移中フラグ
        transitionFlag = true;
        //ボタンを取得
        buttons = gameOverCanvasClone.GetComponentsInChildren<Button>();
        buttons[0].Select();
        // イベントトリガーを取得
        eventTriggers = gameOverCanvasClone.GetComponentsInChildren<EventTrigger>();

        //マウスとキーボード選択を両立したボタン
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

        // ボタンにイベント設定　AfterGameOverの引数により，その後の処理を変える
        buttons[0].onClick.AddListener(() => { AfterGameOver(retry); });
        buttons[1].onClick.AddListener(() => { AfterGameOver(title); });
    }

    public void SelectSelf(Button button)
    {
        button.Select();
    }

    public void NonSelectSelf()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    // ゲームオーバー画面を消し，押したボタンに応じた処理を行う
    public void AfterGameOver(int selectAction)
    {
        Destroy(gameOverCanvasClone);
        if (selectAction == retry) Retry();
        else if (selectAction == title) ReturnTitle();
    }

    // リトライ
    public void Retry()
    {
        // 中間ポイントが存在するかどうか
        if (middlePoint != null)
        {
            // 中間ポイントに達しているか確認
            if (middlePoint.GetComponent<MiddlePointDetector>().middlePointFlag)
                middleResumeFlag = true;
        }
        MoveToStage(currentStageNum);
    }

    // スタート画面に戻る
    public void ReturnTitle()
    {
        currentStageNum = startScene;
        MoveToStage(currentStageNum);
    }

}
