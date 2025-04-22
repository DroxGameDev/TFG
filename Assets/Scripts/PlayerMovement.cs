using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D rb;
    

    [Header("Movement")]
    [Range (0f, 20f)] public float moveSpeed = 5f;
    [Range (0f, 30f)] public float acceleration = 5f;
    [Range (0f, 30f)] public float decceleration = 24f;
    [Range (0f, 1.5f)] public float velPower = 0.9f;

    [Space(10)] 
    [Range (0f, 0.5f)] public float frictionAmount = 0.2f;

    private float moveInput;
    private bool isFacingRight = true;

    [Header("Jumping")]
    [Range (0f, 20f)] public float jumpForce = 15f;
    [Range (0f, 1f)] public float jumpCutMultiplier = 0.5f;
    [Space(10)] 
    [Range (0f, 1f)] public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [Range (0f, 1f)] public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Space(10)] 
    [Range (0f, 5f)] public float gravityScale = 1f;
    [Range (0f, 5f)] public float fallGravityMultiplier = 2f;

    [Space(10)]

    [Header("Check")]
    public Transform groundCheck;
    [Range (0f, 0.5f)] public float groundCheckRadius = 0.1f;
    
    [Space(10)]
    public LayerMask groundLayer;

    private float lastGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void moveInputUpdate(Vector2 context)
    {
        moveInput = context.x;
    }

    public void jumpInputUpdate(bool context)
    {
        if (context){
            jumpBufferCounter = jumpBufferTime;
        }

        if (!context && rb.velocity.y > 0f)
        {
           rb.AddForce(Vector2.down * rb.velocity.y*(1-jumpCutMultiplier), ForceMode2D.Impulse);
           coyoteTimeCounter = 0f;
        }

    }

    void Update()
    {

         //Raycasts para la correcci�n de esquinas

        //rayCast Centro
        RaycastHit2D rayCastUpC = Physics2D.Raycast(transform.position + new Vector3(0f,0.4f), Vector2.up, 0.25f, groundLayer);
        //rayCast Derecha
        RaycastHit2D rayCastUpR = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0.4f), Vector2.up, 0.25f, groundLayer);
        //rayCast Izquierda
        RaycastHit2D rayCastUpL = Physics2D.Raycast(transform.position + new Vector3(-0.2f, 0.4f), Vector2.up, 0.25f, groundLayer);


        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            //correcci�n de esquinas
            if (!rayCastUpC && !rayCastUpR && rayCastUpL)
            {
                transform.position += new Vector3(0.2f, 0);
            }
            else if (!rayCastUpC && rayCastUpR && !rayCastUpL)
            {
                transform.position -= new Vector3(0.2f, 0);
            }

            jumpBufferCounter = 0f;
        }

        //comprobar si hay que girar el sprite
        if (!isFacingRight && moveInput > 0f)
        {
            Flip();
        }
        else if (isFacingRight && moveInput < 0f)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        #region Run
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput * moveSpeed;
        //calculate the difference between our current speed and the target speed
        float speedDif = targetSpeed - rb.velocity.x;
        //change the speed based on the acceleration or decceleration rate
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        //applies acceleration to speed difference, tje raises to a set power so accelerration increases with higher speeds.
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
        #endregion

        #region Friction

        if(Mathf.Abs(moveInput) <0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion

        #region Jump Gravity
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = gravityScale* fallGravityMultiplier;
        }
        else{
            rb.gravityScale = gravityScale;
        }
        #endregion

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}