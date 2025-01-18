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
    [SerializeField]
    float jumpPower = 5f;
    bool isGrounded = false;
    [SerializeField]
    float curtime;
    [SerializeField]
    float cooltime = 0.5f;
 

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
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            ani.SetBool("IsJumping", !isGrounded);
        }

        // 공격 처리
        if (Input.GetKey(KeyCode.Q) && isGrounded)  // Q가 눌려있으면 계속 공격
        {
            if (curtime <= 0)
            {
                ani.SetBool("Attack", true);  // 공격 상태 유지, 시발 루프 때매 한시간을 쓰네 개같은거
                curtime = cooltime;  // 쿨타임 갱신
            }
        }
        else
        {
            ani.SetBool("Attack", false);  // Q를 떼면 공격 상태 종료
        }

        if (curtime > 0)
        {
            curtime -= Time.deltaTime;  // 쿨타임 감소
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        ani.SetFloat("Xvelocity", Math.Abs(rb.velocity.x));
        ani.SetFloat("Yvelocity", rb.velocity.y);
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
        isGrounded = true;
        ani.SetBool("IsJumping", !isGrounded);
    }



}
