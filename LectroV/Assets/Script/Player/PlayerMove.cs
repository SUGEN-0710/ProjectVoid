using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float JumpPower;
    float h;
    bool isDash;
    Rigidbody2D rigid;
    SpriteRenderer SR;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Player Move Control
        if (! anim.GetBool("isRetired")) // isDead?
        {
            if (Input.GetButtonUp("Horizontal"))
            {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.7f, rigid.velocity.y);
            }

            // Player Move Animation
            if (Input.GetButtonDown("Horizontal"))
            {
                SR.flipX = Input.GetAxisRaw("Horizontal") == -1;
            }

            if (rigid.velocity.normalized.x == 0)
            {
                anim.SetBool("isMove", false);
            }
            else
            {
                anim.SetBool("isMove", true);
            }

            // Player Jump Control
            if (Input.GetKeyDown("up") && !anim.GetBool("isJump"))
            {
                rigid.AddForce(Vector2.up * JumpPower, ForceMode2D.Impulse);
                anim.SetBool("isJump", true);
            }


            // Player Aggressive Dash Control
            if (Input.GetKeyDown("v") && Input.GetButton("Horizontal") && !isDash)
            {
                Dash();
            }
        }
    }
   
    void Dash()
    {
        
        rigid.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * 12, ForceMode2D.Impulse);
        isDash = true;
        anim.SetBool("isDash", true);

        Invoke("DashOff", 0.1f);
        
    }

    void DashOff()
    {
        isDash = false;
        anim.SetBool("isDash", false);
        rigid.velocity = new Vector2(0, rigid.velocity.y);
    }
    void FixedUpdate()
    {
        // Player Move Control

        h = Input.GetAxisRaw("Horizontal");
       
        if (!anim.GetBool("isRetired"))
        {
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        

        if (!isDash)
        {
            if (maxSpeed <= rigid.velocity.x)
            {
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }

            if (-maxSpeed >= rigid.velocity.x)
            {
                rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
            }
        }
        
        if (rigid.velocity.x < 0.7f && rigid.velocity.x > 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        if (rigid.velocity.x > -0.7f && rigid.velocity.x < 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        // Player Jump Animation Control
        
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.5f)
            {
                anim.SetBool("isJump", false);
            }
        }

        // if (rigid.velocity.y < 0)

    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Mob")
        {
            if (isDash)
            {
                collision.gameObject.GetComponent<MobAttacked>().Died();
            }
            else
            {
                GameOverStart(collision.gameObject.transform.position);
            }
            
        }

        if (collision.gameObject.tag == "Ground" && anim.GetBool("isRetired"))
        {
            anim.SetBool("isDefeated", true);
        }
    }

    void GameOverStart(Vector2 targetPos)
    {
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;

        SR.color = new Color(1, 1, 1, 0.4f);

        rigid.AddForce(new Vector2(dirc, 1) * 4, ForceMode2D.Impulse);

        anim.SetBool("isRetired", true);

        Invoke("GameOver", 2);
    }
    void GameOver()
    {
        Debug.Log("You Died");
    }

}
