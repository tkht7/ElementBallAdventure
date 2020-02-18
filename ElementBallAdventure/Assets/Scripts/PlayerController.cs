using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    //public float jumpForce;
    public float jumpSpeed;
    public float attackSpeed;
    public Material[] _material;

    public bool attackFlag;
    public Text goalText;
    public int Element; // 0:Normal, 1:Rush, 

    private Image gageImage;
    private Rigidbody rb;
    private ParticleSystem ps;

    private bool onGround;
    private float moveDecayRate;
    private int attackCount;
    private Slider attackGage;
    private float gage;

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody取得
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();

        attackGage = GameObject.Find("AttackGage").GetComponent<Slider>();
        gageImage = GameObject.Find("Fill").GetComponent<Image>();

        rb.maxAngularVelocity = 100;
        
        goalText.text = "";

        onGround = false;
        moveDecayRate = 1.0f;
        attackCount = 0;
        gage = 0.0f;
        attackFlag = false;
        Element = 0;
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
        var movement = new Vector3(moveHorizontal * moveDecayRate, 0, moveVertical * moveDecayRate);
        rb.AddForce(movement * speed * Time.deltaTime);

        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Z) && onGround)
        {
            onGround = false;
            //var moveUp = new Vector3(0, 1, 0);
            //rb.AddForce(moveUp * jumpForce);
            var jumpUp = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            rb.velocity = jumpUp;
            //Debug.Log("call!");
        }

        if(Element == 1)
        {
            Rush(movement);
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

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
            goalText.text = "GOAL!";

        if (collision.gameObject.CompareTag("Rush"))
        {
            //collision.gameObject.transform.root.gameObject.SetActive(false);
            this.GetComponent<Renderer>().material = _material[1];
            Element = 1;
            gage = 1.0f;
            gageImage.color = Color.yellow;
            //gageImage.color = Color.red;
        }
    }

    void Rush(Vector3 movement)
    {
        // 突進
        if (Input.GetKeyDown(KeyCode.X) && gage == 1.0f)
        {
            var attackDirection = movement.normalized;
            if (attackDirection != Vector3.zero)
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(attackDirection * attackSpeed);
                var attackRotation = new Vector3(attackDirection.z, 0, -attackDirection.x);
                rb.AddTorque(attackRotation * Mathf.PI * attackSpeed, ForceMode.Acceleration);

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
            gage += 0.005f;
            if (gage > 1.0f)
                gage = 1.0f;
        }
        attackGage.value = gage;
    }
}
