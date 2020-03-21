﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float playerMoveForce;
    public float[] initialPlayerJumpSpeed;
    public float playerGravity;
    public float playerGravityFall;
    public float rushSpeed;
    public float flameJumpSpeed;
    public Material[] _material;

    public AudioClip rushSound;
    public AudioClip flameSound;
    public AudioClip iceSound;
    public AudioClip itemGetSound;
    private AudioSource audioSource;

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
    private float moveDecayRate;
    private float playerSpeed;
    private const float slowSpeed = 5.0f;
    private const float highSpeed = 15.0f;

    private float playerJumpSpeed;
    private bool useGravity;
    private float localGravity;
    public bool rushFlag;
    private float rushCount;
    private const float rushNoGravityTime = 0.25f;
    private const float rushStateTime = 1.2f;

    private Slider elementGage;
    private float gage;
    private const float fullGage = 1.0f;

    private bool useIce;
    private bool iceSticking;
    private float iceCount;
    private const float iceStickingAcceptTime = 0.1f;

    private GameObject frozenObject;
    private Vector3 FrozenPos;

    private const float rushGageRecoverSpeed = 0.25f;
    private const float flameGageRecoverSpeed = 1.0f;
    private const float iceGageRecoverSpeed = 0.25f;
    private const float iceGageUseSpeed = 0.05f;

    // カメラ位置・角度
    private GameObject playerPos;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        psColor = GetComponent<ParticleSystem>().main;

        elementGage = GameObject.Find("AttackGage").GetComponent<Slider>();
        gageImage = GameObject.Find("Fill").GetComponent<Image>();
        playerPos = GameObject.Find("PlayerPos").gameObject;

        shell1 = transform.Find("IceShell1").gameObject;
        shell2 = transform.Find("IceShell2").gameObject;
        shellCollider1 = shell1.GetComponent<BoxCollider>();
        shellCollider2 = shell2.GetComponent<BoxCollider>();

        rb.maxAngularVelocity = 100; // 最大回転速度の上限を上げる
        
        onGround = false;
        moveDecayRate = 1.0f;
        playerJumpSpeed = 0.0f;
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

    // Update is called once per frame
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



        // スピードテスト
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 testSpeed = new Vector3(0, 0, 50.0f);
            rb.velocity = testSpeed;
        }
        Debug.Log(rb.velocity);


        //Debug.Log(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // 重力をAddForceでかけるメソッドを呼ぶ。
        if(useGravity)
            SetLocalGravity(localGravity); 
    }

    // プレイヤーに重力をかける
    private void SetLocalGravity(float gravity)
    {
        rb.AddForce(new Vector3(0.0f, gravity, 0.0f), ForceMode.Acceleration);
    }

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
        movement = new Vector3(moveHorizontal * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    + moveVertical * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate,
                                    0.0f,
                                    moveVertical * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    - moveHorizontal * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate);
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
            rb.velocity = new Vector3(rb.velocity.x, playerJumpSpeed, rb.velocity.z);
        }
        // ボタンを離すか，降下中は重力を強くする
        else if (Input.GetKeyUp(KeyCode.Z) || rb.velocity.y < 0.0f)
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
    }

    void FlameAction()
    {
        if (Input.GetKeyDown(KeyCode.X) && gage == fullGage)
        {
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
    }

    void IceAction()
    {
        // 自身の回りに氷を張る
        if (Input.GetKeyDown(KeyCode.X) && gage > 0.0f)
            CreateIce();

        // 氷を張っているときの処理
        if (useIce)
            UseIceProcess();

        // 張り付き解除
        if (Input.GetKeyUp(KeyCode.X) || gage <= 0.0f)
            IceStickingCancel();


        // 何かに張り付いているときの処理
        if (iceSticking)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 張り付いた時，その物体と一緒に動く
            IceStickingMove();
        }

        // 地上にいるときゲージ回復
        if (onGround && !useIce)
            GageRecover(iceGageRecoverSpeed);
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

    // 氷を張っているときの処理
    void UseIceProcess()
    {
        // 氷を張った瞬間(iceStickingAcceptTime以内)に物体に触れてなかったら張り付けない
        if (!iceSticking && iceCount >= iceStickingAcceptTime)
        {
            shellCollider1.isTrigger = false;
            shellCollider2.isTrigger = false;
        }
        // 氷を張ってからの時間を計測
        iceCount += Time.deltaTime;

        gage -= iceGageUseSpeed * Time.deltaTime;
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
        elementGage.value = gage;
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

        // 張り付けるかどうかの判定 (触れている物体が透過する場合は張り付かない)
        if (useIce && iceCount < iceStickingAcceptTime && !collider.isTrigger)
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
