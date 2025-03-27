using System;
using UnityEngine;


public class Player : MonoBehaviour
{
    float horizontalInput;
    bool facingRight = true;
    bool isGrounded = false;
    bool isDashing = false;
    bool isDefending = false;
    bool isS_Attack = false;

    [SerializeField] float S_AttackForce = 20f; // 특수기 위로 뜨는 힘 
    [SerializeField] float moveSpeed = 5f; // 이동 속도 
    [SerializeField] float jumpPower = 5f; // 점프 힘 
    [SerializeField] int maxJumpCount = 2; // 최대 점프 횟수
    [SerializeField] float dashSpeed = 10f; // 대쉬 속도 
    [SerializeField] float dashDuration = 0.2f; // 대쉬 지속 시간
    [SerializeField] float dashCooldown = 1f; // 대쉬 쿨타임
    [SerializeField] float defendCooldown = 1f; // 패링 쿨타임 (증가)
    [SerializeField] float defendDuration = 0.15f; // 패링 지속 시간
    [SerializeField] float cooltime = 0.5f; // 쿨타임 

    int jumpCount = 0; // 점프 횟수 추적
    float dashTime = 0;
    float dashCooldownTime = 0;
    float defendCooldownTime = 0;
    float defendTime = 0;
    float curtime;

    
    Rigidbody2D rb;
    Animator ani;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }



    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // 좌우 이동 처리 

        FlipSprite();
        Jump();
        Attack();
        Dash();
        Defend();
        

    }



    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2(facingRight ? dashSpeed : -dashSpeed, rb.velocity.y);
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                ani.SetBool("Dash", false);
            }
        }
        else
        {
            float AdjustSpeed = moveSpeed;

            if (ani.GetBool("Attack"))
            {
                AdjustSpeed *= 0.3f;
            }
            if (ani.GetBool("S_Attack"))
            {
                AdjustSpeed *= 0.3f;
                rb.AddForce(Vector3.up * S_AttackForce, ForceMode2D.Impulse);
            }
            if (ani.GetBool("IsJumping"))
            {
                AdjustSpeed *= 0.7f;
            }
            rb.velocity = new Vector2(horizontalInput * AdjustSpeed, rb.velocity.y);
            ani.SetFloat("Xvelocity", Math.Abs(rb.velocity.x));
            ani.SetFloat("Yvelocity", rb.velocity.y);
        }

        // 대쉬 쿨타임 감소
        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.fixedDeltaTime;
        }

        // 방어 지속 시간 및 쿨타임 처리
        if (isDefending)
        {
            defendTime -= Time.fixedDeltaTime;
            if (defendTime <= 0)
            {
                isDefending = false;
                ani.SetBool("Defend", false);
            }
        }

        if (defendCooldownTime > 0)
        {
            defendCooldownTime -= Time.fixedDeltaTime;
        }

       
    }

    void Attack() // 공격 & 공중 공격 처리
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (curtime <= 0)
            {
                if (isGrounded)
                {
                    ani.SetBool("Air_Attack", false);
                    curtime = cooltime;
                    ani.SetBool("Attack", true);
                }
                else
                {
                    ani.SetBool("Attack", false);
                    curtime = cooltime;
                    ani.SetBool("Air_Attack", true);
                }
                curtime = cooltime;
            }
        }

        else if (Input.GetKeyDown(KeyCode.W)) // 특수 공격기 
        {
            ani.SetBool("S_Attack", true);
        }

        else
        {
            ani.SetBool("S_Attack", false);
            ani.SetBool("Attack", false);
            ani.SetBool("Air_Attack", false);
        }

        if (curtime > 0)
        {
            curtime -= Time.deltaTime;
        }
    }

    void Jump() // 점프 처리 
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpCount++;
            ani.SetBool("IsJumping", true);
            isGrounded = false;
        }
    }

    void Defend() // 패링 기능 처리 
    {
        if (Input.GetKeyDown(KeyCode.E) && defendCooldownTime <= 0 && !isDefending && !isDashing)
        {
            isDefending = true;
            defendTime = defendDuration;
            defendCooldownTime = defendCooldown;
            ani.SetBool("Defend", true);
        }
    }

    void Dash() // 대쉬 기능 처리 
    {
        if (Input.GetKeyDown(KeyCode.R) && dashCooldownTime <= 0 && !isDashing && !isDefending)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashCooldownTime = dashCooldown;
            ani.SetBool("Dash", true);
        }
    }

    

    void FlipSprite()
    {
        if (facingRight && horizontalInput < 0f || !facingRight && horizontalInput > 0f)
        {
            facingRight = !facingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }





    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            ani.SetBool("IsJumping", false);
            ani.SetBool("Air_Attack", false);
        }
    }
}