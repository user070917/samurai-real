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
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpCount++;
            ani.SetBool("IsJumping", true);
        }

        // 공격 처리
        if (Input.GetKey(KeyCode.Q) && isGrounded) // Q가 눌려있으면 계속 공격
        {
            if (curtime <= 0)
            {
                ani.SetBool("Attack", true); // 공격 상태 유지
                curtime = cooltime; // 쿨타임 갱신
            }
        }
        //else if (ani.GetBool("IsJumping") && ani.GetBool("Attack"))  // air attack
        else if (Input.GetKey(KeyCode.W))
        {
            if (curtime <= 0)
            {
                ani.SetBool("Air_Attack", true); // air attack 활성화 
                curtime = cooltime; // 쿨타임 갱신
            }
        }
      
        else if ()
        {
            ani.SetBool("Air_Attack", false); // 땅에 닿거나 Q를 누르고 있지 않으면 상태 종료 
            ani.SetBool("Attack", false); // Q를 떼면 공격 상태 종료
        }

        if (curtime > 0)
        {
            curtime -= Time.deltaTime; // 쿨타임 감소
        }
    }

    private void FixedUpdate()
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
        // 땅에 닿았을 때 점프 카운트 초기화
        isGrounded = true;
        jumpCount = 0;
        ani.SetBool("IsJumping", false);
    }
}
