using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private PlayerData playerData;
    

    [Header("Movement")]
    private float moveInput;
    private bool isFacingRight = true;

    [Header("Jumping")]
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Debug")]
    public bool active;
    private Vector3 groundPosition;
    private float groundRadius;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();

        //selecteMetalThreshold = new Vector2(0.1f,0.1f);
    }

    #region Input
    public void moveInputUpdate(Vector2 context)
    {
        moveInput = context.x;
    }

    public void jumpInputUpdate(bool context)
    {
        if (context){
            jumpBufferCounter = playerData.jumpBufferTime;
        }

        if (!context && rb.velocity.y > 0f)
        {
           rb.AddForce(Vector2.down * rb.velocity.y*(1-playerData.jumpCutMultiplier), ForceMode2D.Impulse);
           coyoteTimeCounter = 0f;
        }
        
    }

    

    #endregion

    void Update()
    {
        
        if (IsGrounded())
        {
            playerData.grounded = true;
            coyoteTimeCounter = playerData.coyoteTime;
        }
        else
        {
            playerData.grounded = false;
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter -= Time.deltaTime;

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
            
            jumpBufferCounter = 0f;
        }

        #region Animation States
        if(Mathf.Abs(moveInput) > 0f){
            playerData.running = true;
        }
        else{
            playerData.running = false;
        }
        if (Mathf.Abs(rb.velocity.y) == 0f || playerData.grounded)
        {
            playerData.falling = false;
            playerData.jumping = false;
        }
        else if (rb.velocity.y > 0f){
            playerData.jumping = true;
            playerData.falling = false;
        }
        else{
            playerData.jumping = false;
            playerData.falling = true;
        }
        #endregion

        //comprobar si hay que girar el sprite
        if (!isFacingRight && moveInput > 0f)
        {
            Flip();
        }
        else if (isFacingRight && moveInput < 0f)
        {
            Flip();
        }

        #region debug
        groundPosition = playerData.groundCheck.position;
        groundRadius = playerData.groundCheckRadius;
        #endregion
        
    }

    void FixedUpdate()
    {
        if (!playerData.movingWithPowers){
            #region Run
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = moveInput * playerData.moveSpeed;
            //calculate the difference between our current speed and the target speed
            float speedDif = targetSpeed - rb.velocity.x;
            //change the speed based on the acceleration or decceleration rate
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.acceleration : playerData.decceleration;
            //applies acceleration to speed difference, tje raises to a set power so accelerration increases with higher speeds.
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, playerData.velPower) * Mathf.Sign(speedDif);

            rb.AddForce(movement * Vector2.right);
            #endregion
        }

        #region Friction

        if(Mathf.Abs(moveInput) <0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerData.frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion

        GravityController(playerData.falling, playerData.jumping);

    }

    public void GravityController(bool isFalling, bool isJumping){
        if ((isJumping || isFalling) && Mathf.Abs(rb.velocity.y) < playerData.jumpHangTheshold){
            setGravityScale(playerData.gravityScale*playerData.jumpHangMultiplier);
        }  
        else if (rb.velocity.y < 0f)
        {
            setGravityScale(playerData.gravityScale* playerData.fallGravityMultiplier);
        }
        else{
            setGravityScale(playerData.gravityScale);
        }
    }

    private void setGravityScale(float newGravity){
        rb.gravityScale = newGravity;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    private void Flip()
    {
        if (!playerData.timeStoped){
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmos()
    {
        if(active){
            if (active){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundPosition,groundRadius);
            }
        }
    }


}