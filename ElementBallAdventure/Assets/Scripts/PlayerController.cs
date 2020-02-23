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

    public bool attackFlag;
    public Text goalText;
    public int Element; // 0:Normal, 1:Rush, 2:Flame

    private Image gageImage;
    private Rigidbody rb;
    private ParticleSystem ps;

    private bool onGround;
    private float moveDecayRate;
    private int attackCount;
    private Slider attackGage;
    private float gage;
    private float accel;

    private GameObject playerPos;

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody取得
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();

        attackGage = GameObject.Find("AttackGage").GetComponent<Slider>();
        gageImage = GameObject.Find("Fill").GetComponent<Image>();
        playerPos = GameObject.Find("PlayerPos").gameObject;

        rb.maxAngularVelocity = 100;
        
        goalText.text = "";

        onGround = false;
        moveDecayRate = 1.0f;
        attackCount = 0;
        gage = 0.0f;
        attackFlag = false;
        Element = 0;
        accel = 1.0f;
        
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
        var movement = new Vector3(moveHorizontal * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180) * moveDecayRate
                                    + moveVertical * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180) * moveDecayRate,
                                    0, 
                                    moveVertical * Mathf.Cos(playerPos.transform.localEulerAngles.y * Mathf.PI / 180) * moveDecayRate
                                    - moveHorizontal * Mathf.Sin(playerPos.transform.localEulerAngles.y * Mathf.PI / 180) * moveDecayRate);
        rb.AddForce(movement * speed * Time.deltaTime);

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && onGround)
        {
            onGround = false;
            var jumpUp = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            rb.velocity = jumpUp;
        }

        if(Element == 1)
        {
            Rush(movement);
        }
        else if (Element == 2)
        {
            Flame();
        }


        // スピードテスト
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Vector3 testSpeed = new Vector3(0, 0, 50.0f);
        //    rb.velocity = testSpeed;
        //}
        //Debug.Log(rb.velocity);
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
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
            ps.startColor = Color.yellow;
        }
        else if (collider.gameObject.CompareTag("Flame"))
        {
            this.GetComponent<Renderer>().material = _material[2];
            Element = 2;
            gage = 1.0f;
            gageImage.color = Color.red;
            ps.startColor = Color.red;
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
                var attackRotation = new Vector3(rushDirection.z, 0, -rushDirection.x);
                rb.AddTorque(attackRotation * Mathf.PI * rushSpeed, ForceMode.Acceleration);

                gage = 0.0f;
                attackFlag = true;

                // エフェクト
                ps.Play();
            }
        }

        // 突進から一定時間は重力の影響を受けない
        if (attackCount >= 15)
        {
            rb.useGravity = true;
        }

        // 突進から一定時間，突進を使ってることのフラグを立てる
        if (attackCount >= 70)
        {
            attackFlag = false;
            attackCount = 0;
        }

        if (attackFlag)
        {
            attackCount++;
        }

        // 時間経過でゲージの回復
        if (gage < 1.0f)
        {
            gage += 0.004f;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        attackGage.value = gage;
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
            gage += 0.02f;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        attackGage.value = gage;
    }

}
