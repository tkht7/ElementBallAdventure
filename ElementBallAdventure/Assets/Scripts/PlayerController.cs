using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float rushSpeed;
    public float flameJumpSpeed;
    public Material[] _material;

    public bool rushFlag;
    public Text goalText;
    public int Element; // 0:Normal, 1:Rush, 2:Flame, 3:Ice

    private Image gageImage;
    private Rigidbody rb;
    private ParticleSystem ps;
    private ParticleSystem.MainModule psColor;
    private GameObject shell1;
    private GameObject shell2;
    private BoxCollider shellCollider1;
    private BoxCollider shellCollider2;

    private bool onGround;
    private float moveDecayRate;
    private float rushCount;
    private Slider elementGage;
    private float gage;
    private float accel;
    private bool useIceStart;
    private bool useIce;
    private bool iceFlag;
    private float iceCount;
    private GameObject frozenObject;
    private Vector3 FrozenPos;

    private GameObject playerPos;

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody取得
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

        rb.maxAngularVelocity = 100;

        goalText.text = "";

        onGround = false;
        moveDecayRate = 1.0f;
        rushCount = 0.0f;
        gage = 0.0f;
        rushFlag = false;
        Element = 0;
        accel = 1.0f;
        useIceStart = false;
        useIce = false;
        iceFlag = false;
        iceCount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (onGround)
            moveDecayRate = 1.0f;
        else
            moveDecayRate = 0.5f; // ジャンプ中は移動量減少

        // 移動
        var moveHorizontal = Input.GetAxis("Horizontal");
        var moveVertical = Input.GetAxis("Vertical");
        var movement = new Vector3(moveHorizontal * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    + moveVertical * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate,
                                    0,
                                    moveVertical * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate
                                    - moveHorizontal * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180.0f) * moveDecayRate);
        if (!useIce)
            rb.AddForce(movement * speed * Time.deltaTime);


        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && onGround && !useIce)
        {
            onGround = false;
            var jumpUp = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            rb.velocity = jumpUp;
        }

        if (Element == 1)
        {
            Rush(movement);
        }
        else if (Element == 2)
        {
            Flame();
        }
        else if (Element == 3)
        {
            Ice();
        }


        // スピードテスト
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Vector3 testSpeed = new Vector3(0, 0, 50.0f);
        //    rb.velocity = testSpeed;
        //}
        //Debug.Log(rb.velocity);

        //Debug.Log(useIceStart);
        //Debug.Log(useIce);
        //Debug.Log(iceFlag);
        Debug.Log(Time.deltaTime);
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
        if (collider.gameObject.CompareTag("Goal"))
            goalText.text = "GOAL!";

        if (collider.gameObject.CompareTag("Rush"))
        {
            this.GetComponent<Renderer>().material = _material[1];
            Element = 1;
            gage = 1.0f;
            gageImage.color = Color.yellow;
            psColor.startColor = Color.yellow;
        }
        else if (collider.gameObject.CompareTag("Flame"))
        {
            this.GetComponent<Renderer>().material = _material[2];
            Element = 2;
            gage = 1.0f;
            gageImage.color = Color.red;
            psColor.startColor = Color.red;
        }
        else if (collider.gameObject.CompareTag("Ice"))
        {
            this.GetComponent<Renderer>().material = _material[3];
            Element = 3;
            gage = 1.0f;
            gageImage.color = Color.cyan;
            psColor.startColor = Color.cyan;
        }

        if (useIceStart && !collider.gameObject.CompareTag("WindZone"))
        {
            iceFlag = true;
            rb.isKinematic = true;
            frozenObject = collider.gameObject;
            FrozenPos = frozenObject.transform.position;
        }

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Accel"))
        {
            var tempV = rb.velocity;
            rb.velocity = new Vector3(tempV.x, tempV.y, tempV.z + accel); // +Z以外に進むやつがいる時はrotationから向きを求めて作る
        }
    }

    void Rush(Vector3 movement)
    {
        // 突進
        if (Input.GetKeyDown(KeyCode.X) && gage == 1.0f)
        {
            var rushDirection = movement.normalized;
            if (rushDirection != Vector3.zero)
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(rushDirection * rushSpeed);
                var rushRotation = new Vector3(rushDirection.z, 0, -rushDirection.x);
                rb.AddTorque(rushRotation * Mathf.PI * rushSpeed, ForceMode.Acceleration);

                gage = 0.0f;
                rushFlag = true;

                // エフェクト
                ps.Play();
            }
        }

        // 突進から一定時間は重力の影響を受けない
        if (rushCount >= 0.25f)
        {
            rb.useGravity = true;
        }

        // 突進から一定時間，突進を使ってることのフラグを立てる
        if (rushCount >= 1.2f)
        {
            rushFlag = false;
            rushCount = 0.0f;
        }

        if (rushFlag)
        {
            rushCount += Time.deltaTime;
        }

        // 時間経過でゲージの回復
        if (gage < 1.0f)
        {
            gage += Time.deltaTime * 0.25f;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        elementGage.value = gage;
    }

    void Flame()
    {
        // 大ジャンプ
        if (Input.GetKeyDown(KeyCode.X) && gage == 1.0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, flameJumpSpeed, rb.velocity.z);

            gage = 0.0f;

            // エフェクト
            ps.Play();

        }

        // 地上で時間経過でゲージの回復
        if (gage < 1.0f && onGround)
        {
            gage += Time.deltaTime;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        elementGage.value = gage;
    }

    void Ice()
    {
        // 自身の回りに氷を張る
        if (Input.GetKeyDown(KeyCode.X) && gage > 0.0f)
        {
            shell1.SetActive(true);
            shell2.SetActive(true);

            useIceStart = true;
            useIce = true;

            // エフェクト いらないかも
            //ps.Play();

        }
        // 氷を張った瞬間(iceCount0.1f以内)に物体に触れてなかったら張り付けない
        else if (Input.GetKey(KeyCode.X) && gage > 0.0f && shell1.activeSelf)
        {
            iceCount += Time.deltaTime;
            if (!iceFlag && useIceStart && iceCount >= 0.1f)
            {
                shellCollider1.isTrigger = false;
                shellCollider2.isTrigger = false;
                useIceStart = false;
            }
            gage -= Time.deltaTime * 0.05f;

        }

        // 張り付き解除
        if (Input.GetKeyUp(KeyCode.X) || gage <= 0.0f)
        {
            useIceStart = false;
            useIce = false;
            iceFlag = false;
            shellCollider1.isTrigger = true;
            shellCollider2.isTrigger = true;
            rb.isKinematic = false;
            shell1.SetActive(false);
            shell2.SetActive(false);
            iceCount = 0.0f;
        }

        // 張り付いた時，その物体と一緒に動く
        if (iceFlag)
        {
            var preFrozenPos = FrozenPos;
            FrozenPos = frozenObject.transform.position;
            transform.position = new Vector3(transform.position.x + (FrozenPos.x - preFrozenPos.x),
                                             transform.position.y + (FrozenPos.y - preFrozenPos.y),
                                             transform.position.z + (FrozenPos.z - preFrozenPos.z));
        }

        // 地上で時間経過でゲージの回復
        if (gage < 1.0f && onGround && !useIce)
        {
            gage += Time.deltaTime * 0.25f;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        elementGage.value = gage;
    }
}
