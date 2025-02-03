using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float horizontalInput;
    [SerializeField]
    float moveSpeed = 5f;
    bool facingRight = true;
    bool isGrounded = false;
    [SerializeField]
    float jumpPower = 5f;
    int jumpCount = 0; // 점프 횟수 추적
    [SerializeField]
    int maxJumpCount = 2; // 최대 점프 횟수
    [SerializeField]
    float dashSpeed = 10f;
    [SerializeField]
    float dashDuration = 0.2f; // 대쉬 지속 시간
    [SerializeField]
    float dashCooldown = 1f; // 대쉬 쿨타임

    bool isDashing = false;
    float dashTime = 0;
    float dashCooldownTime = 0;

    float curtime;
    [SerializeField]
    float cooltime = 0.5f;
    int maxhp = 100; //최대 체력 

    Rigidbody2D rb;
    Animator ani;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        // 점프 처리
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpCount++;
            ani.SetBool("IsJumping", true);
            isGrounded = false;
        }

        // 공격 처리
        if (Input.GetKey(KeyCode.Q)) // Q 키를 누르면 공격 실행
        {
            if (curtime <= 0)
            {
                if (isGrounded)
                {
                    ani.SetBool("Attack", true); // 지상 공격
                }
                else
                {
                    ani.SetBool("Air_Attack", true); // 공중 공격
                }
                curtime = cooltime; // 쿨타임 갱신
            }
        }
        else
        {
            ani.SetBool("Attack", false);
            ani.SetBool("Air_Attack", false); // 키를 떼면 공중 공격도 해제
        }

        if (curtime > 0)
        {
            curtime -= Time.deltaTime; // 쿨타임 감소
        }


        if (Input.GetKeyDown(KeyCode.R) && dashCooldownTime <= 0 && !isDashing)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashCooldownTime = dashCooldown;
            ani.SetBool("Dash", true);
        }
        //else
        //{
        //    ani.SetBool("Dash", false);
        //}
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
            ani.SetBool("IsJumping", false);
            ani.SetBool("Air_Attack", false); // 바닥에 닿으면 공중 공격 해제
        }
    }
}
