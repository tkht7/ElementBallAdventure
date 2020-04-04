using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float playerMoveForce;          // 移動時に加える力の大きさ
    public float[] initialPlayerJumpSpeed; // 速度ごとのジャンプの初速
    public float playerGravity;            // 通常時の重力
    public float playerGravityFall;        // 降下時の重力
    public float rushSpeed;                // 突進使用時の速さ
    public float flameJumpSpeed;           // 大ジャンプのジャンプスピード
    public Material[] _material;           // 属性ごとの見た目

    public AudioClip rushSound;
    public AudioClip flameSound;
    public AudioClip iceSound;
    public AudioClip itemGetSound;
    private AudioSource audioSource;

    // プレイヤーの状態　属性ごと
    private int playerElement;
    private const int normalElement = 0;
    private const int rushElement = 1;
    private const int flameElement = 2;
    private const int iceElement = 3;

    private Image gageImage;
    private Rigidbody rb;
    private ParticleSystem ps;
    private ParticleSystem.MainModule psColor;
    private GameObject shell1;
    private GameObject shell2;
    private BoxCollider shellCollider1;
    private BoxCollider shellCollider2;

    private bool onGround;
    private Vector3 movement;

    // ジャンプ中に加わる移動のための力は半分になる
    private float moveDecayRate;
    private const float groundMoveDecayRate = 1.0f;
    private const float jumpMoveDecayRate = 0.5f;

    // プレイヤーのスピードによりジャンプの大きさが変わる
    // slowSpeed未満，highSpeed未満，highSpeed以上の三つ
    private float playerSpeed;
    private const float slowSpeed = 5.0f;
    private const float highSpeed = 15.0f;
    private float playerJumpSpeed;

    // どのジャンプを使っているか
    // 使用してない，ジャンプ，大ジャンプ
    private int jumpState;
    private const int notJump = 0;
    private const int normalJump = 1;
    private const int flameJump = 2;
    
    // 重力使用判定
    private bool useGravity;
    private float localGravity;
    
    private float rushCount;
    private const float rushNoGravityTime = 0.25f; // 突進時の重力無効時間
    private const float rushStateTime = 1.2f;      // 突進状態であると判断される時間
    public bool rushFlag;                          // 突進状態であるかどうか

    private Slider elementGage;
    private float gage;
    private const float fullGage = 1.0f; // ゲージ満タン時の数値

    private bool useIce;
    private bool iceSticking;
    private float iceCount;
    private const float iceStickingAcceptTime = 0.2f;

    private GameObject frozenObject;
    private Vector3 FrozenPos;

    // 各属性のゲージ回復速度・消費速度
    private const float rushGageRecoverSpeed = 0.25f;
    private const float flameGageRecoverSpeed = 1.0f;
    private const float iceGageRecoverSpeed = 0.25f;
    private const float iceGageUseSpeed = 0.05f;

    // カメラ位置・角度
    private GameObject cameraRelation;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        psColor = GetComponent<ParticleSystem>().main;

        elementGage = GameObject.Find("AttackGage").GetComponent<Slider>();
        gageImage = GameObject.Find("Fill").GetComponent<Image>();
        cameraRelation = GameObject.Find("CameraRelation").gameObject;

        shell1 = transform.Find("IceShell1").gameObject;
        shell2 = transform.Find("IceShell2").gameObject;
        shellCollider1 = shell1.GetComponent<BoxCollider>();
        shellCollider2 = shell2.GetComponent<BoxCollider>();

        rb.maxAngularVelocity = 100; // 最大回転速度の上限を上げる

        onGround = false;
        moveDecayRate = groundMoveDecayRate;
        playerJumpSpeed = 0.0f;
        jumpState = notJump;
        useGravity = true;
        localGravity = playerGravity;
        rushCount = 0.0f;
        gage = 0.0f;
        rushFlag = false;
        playerElement = normalElement;
        useIce = false;
        iceSticking = false;
        iceCount = 0.0f;
    }
    
    void Update()
    {
        // 移動量の調整
        SetMoveDecayRate();

        // 移動
        playerMoveProcess();

        // ジャンプの準備
        if(onGround) playerJumpPrepare();

        // ジャンプ
        playerJumpProcess();

        // 各属性のアクション
        ElementAction();
    }

    private void FixedUpdate()
    {
        // 重力をAddForceでかけるメソッドを呼ぶ。
        if(useGravity)
            SetLocalGravity(localGravity);

        // 何かに張り付いているときの処理
        if (iceSticking)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 張り付いた時，その物体と一緒に動く
            IceStickingMove();
        }
    }

    // プレイヤーに重力をかける
    private void SetLocalGravity(float gravity)
    {
        rb.AddForce(new Vector3(0.0f, gravity, 0.0f), ForceMode.Acceleration);
    }

    // 移動量の係数
    void SetMoveDecayRate()
    {
        if (onGround) moveDecayRate = 1.0f;
        else          moveDecayRate = 0.5f; // ジャンプ中は移動量減少
    }

    // 移動
    void playerMoveProcess()
    {
        var moveHorizontal = 0.0f;
        var moveVertical = 0.0f;

        // 前後左右の移動方向を決める。対になるボタンの同時押しでは何も起こらない。
        if (Input.GetKey(KeyCode.RightArrow)) moveHorizontal += 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow))  moveHorizontal -= 1.0f;
        if (Input.GetKey(KeyCode.UpArrow))    moveVertical += 1.0f;
        if (Input.GetKey(KeyCode.DownArrow))  moveVertical -= 1.0f;

        // カメラの向きに合わせて移動方向を変更
        movement = new Vector3(moveHorizontal * Mathf.Cos(cameraRelation.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    + moveVertical * Mathf.Sin(cameraRelation.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate,
                                    0.0f,
                                    moveVertical * Mathf.Cos(cameraRelation.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    - moveHorizontal * Mathf.Sin(cameraRelation.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate);
        if (!useIce)
            rb.AddForce(movement * playerMoveForce * Time.deltaTime);
    }

    // ジャンプの準備
    void playerJumpPrepare()
    {
        localGravity = playerGravity;
        playerSpeed = rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z;
        if (playerSpeed < slowSpeed * slowSpeed)
        {
            playerJumpSpeed = initialPlayerJumpSpeed[0];
        }
        else if (playerSpeed < highSpeed * highSpeed)
        {
            playerJumpSpeed = initialPlayerJumpSpeed[1];
        }
        else
        {
            playerJumpSpeed = initialPlayerJumpSpeed[2];
        }
    }

    // ジャンプ
    void playerJumpProcess()
    {
        if (Input.GetKeyDown(KeyCode.Z) && onGround && !useIce)
        {
            onGround = false;
            jumpState = normalJump;
            rb.velocity = new Vector3(rb.velocity.x, playerJumpSpeed, rb.velocity.z);
        }
        // 普通のジャンプ中にボタンを離すか，降下中は重力を強くする
        else if (jumpState == normalJump && Input.GetKeyUp(KeyCode.Z) || rb.velocity.y < 0.0f)
        {
            localGravity = playerGravityFall;
        }
    }

    // 各属性のアクション
    void ElementAction()
    {
        if (playerElement == rushElement)
        {
            RushAction();
        }
        else if (playerElement == flameElement)
        {
            FlameAction();
        }
        else if (playerElement == iceElement)
        {
            IceAction();
        }
    }

    void RushAction()
    {
        if (Input.GetKeyDown(KeyCode.X) && gage == fullGage)
        {
            var rushDirection = movement.normalized;
            if (rushDirection != Vector3.zero)
            {
                // 突進から一定時間は重力の影響を受けない
                useGravity = false;

                rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
                rb.AddForce(rushDirection * rushSpeed);
                var rushRotation = new Vector3(rushDirection.z, 0.0f, -rushDirection.x);
                rb.AddTorque(rushRotation * Mathf.PI * rushSpeed, ForceMode.Acceleration);

                // ゲージを一気に消費
                gage = 0.0f;

                // 突進を使ってからの時間計測用
                rushCount = 0.0f;

                // 一定時間，突進を使ってることのフラグを立てる
                rushFlag = true;

                // 効果音
                audioSource.PlayOneShot(rushSound);

                // エフェクト
                ps.Play();
            }
        }
        
        if (rushCount >= rushNoGravityTime) useGravity = true;
        if (rushCount >= rushStateTime) rushFlag = false;

        // 突進を使ってからの時間を計測
        if (rushFlag) rushCount += Time.deltaTime;

        // 時間経過でゲージ回復
        GageRecover(rushGageRecoverSpeed);
        elementGage.value = gage;
    }

    void FlameAction()
    {
        if (Input.GetKeyDown(KeyCode.X) && gage == fullGage)
        {
            jumpState = flameJump;
            rb.velocity = new Vector3(rb.velocity.x, flameJumpSpeed, rb.velocity.z);
            gage = 0.0f;
            localGravity = playerGravity;

            // 効果音
            audioSource.PlayOneShot(flameSound);

            // エフェクト
            ps.Play();
        }
        // 地上にいるときゲージ回復
        if (onGround) GageRecover(flameGageRecoverSpeed);
        elementGage.value = gage;
    }

    void IceAction()
    {
        // 氷を張っているときの処理
        if (useIce)
            UseIceProcess();

        // 自身の回りに氷を張る
        if (Input.GetKeyDown(KeyCode.X) && gage > 0.0f)
            CreateIce();

        // 張り付き解除
        if (Input.GetKeyUp(KeyCode.X) || gage <= 0.0f)
            IceStickingCancel();

        // 地上にいるときゲージ回復
        if (onGround && !useIce)
            GageRecover(iceGageRecoverSpeed);
        elementGage.value = gage;
    }

    // 氷を張っているときの処理
    void UseIceProcess()
    {
        // 氷を張った瞬間(iceCount0.1f以内)に物体に触れてなかったら張り付けない
        if (!iceSticking && iceCount >= iceStickingAcceptTime)
        {
            shellCollider1.isTrigger = false;
            shellCollider2.isTrigger = false;
        }
        // 氷を張ってからの時間を計測
        iceCount += Time.deltaTime;

        gage -= iceGageUseSpeed * Time.deltaTime;
    }

    // 自身の回りに氷を張る
    void CreateIce()
    {
        shell1.SetActive(true);
        shell2.SetActive(true);
        useIce = true;

        // 効果音
        audioSource.PlayOneShot(iceSound);

        // エフェクト　ないほうが良いかも
        //ps.Play();
    }

    // 張り付き解除
    void IceStickingCancel()
    {
        useIce = false;
        iceSticking = false;
        shellCollider1.isTrigger = true;
        shellCollider2.isTrigger = true;
        useGravity = true;
        shell1.SetActive(false);
        shell2.SetActive(false);
        iceCount = 0.0f;
    }

    // 張り付いた物体と一緒に動く
    void IceStickingMove()
    {
        var preFrozenPos = FrozenPos;
        FrozenPos = frozenObject.transform.position;
        rb.position = new Vector3(rb.position.x + (FrozenPos.x - preFrozenPos.x),
                                  rb.position.y + (FrozenPos.y - preFrozenPos.y),
                                  rb.position.z + (FrozenPos.z - preFrozenPos.z));
    }

    // ゲージ回復 (回復の早さ)
    void GageRecover(float recoverSpeed)
    {
        if (gage < fullGage)
        {
            gage += recoverSpeed * Time.deltaTime;
            if (gage > fullGage)
                gage = fullGage;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            onGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            onGround = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        // 各属性へ変身
        if (collider.gameObject.CompareTag("Rush"))       ChangeElement(rushElement, Color.yellow, Color.yellow);
        else if (collider.gameObject.CompareTag("Flame")) ChangeElement(flameElement, Color.red, Color.red);
        else if (collider.gameObject.CompareTag("Ice"))   ChangeElement(iceElement, Color.cyan, Color.cyan);

        // 氷を張った時，張り付けるかどうかの判定 (触れている物体が透過する場合は張り付かない)
        if (useIce && iceCount < iceStickingAcceptTime && !iceSticking && !collider.isTrigger)
            ApplyIceSticking(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        // 張り付いてる物体から引き剥がされたら，張り付き解除
        if (iceSticking && collider.gameObject == frozenObject)
            IceStickingCancel();
    }

    // 各属性へ変身
    void ChangeElement(int element, UnityEngine.Color gageColor, UnityEngine.Color effectColor)
    {
        this.GetComponent<Renderer>().material = _material[element];
        playerElement = element;
        gage = fullGage;
        gageImage.color = gageColor;
        psColor.startColor = effectColor;
        audioSource.PlayOneShot(itemGetSound);
    }

    // 張り付き状態にする
    void ApplyIceSticking(Collider collider)
    {
        iceSticking = true;
        useGravity = false;
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.Euler(Vector3.zero);
        frozenObject = collider.gameObject;
        FrozenPos = frozenObject.transform.position;
    }
}
