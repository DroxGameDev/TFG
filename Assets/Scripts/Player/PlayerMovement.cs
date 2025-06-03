using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : AffectedByGravity
{
    private PlayerData playerData;
    private ConstantForce2D force2D;
    

    [Header("Movement")]
    private float moveInput;

    [Header("Jumping")]
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Debug")]
    public bool active;
    private Vector3 groundPosition;
    private float groundRadius;

    void Start()
    {
        //OnStart();
        playerData = GetComponent<PlayerData>();
        //selecteMetalThreshold = new Vector2(0.1f,0.1f);
    }

    #region Input
    public void MoveInputUpdate(Vector2 context)
    {
       moveInput = context.x;
    }

    public void JumpInputUpdate(bool context)
    {
        if (context)
        {
            jumpBufferCounter = playerData.jumpBufferTime;
            
            if (playerData.gravityMode != GravityMode.Down){
                rb.velocity = Vector2.zero;
                playerData.ChangeGravityMode(GravityMode.Down);
            }
        }

        if (!context && playerData.velocity.y > 0f)
        {
            //CancelPowerMovement();
           rb.AddForce(Vector2.down * playerData.velocity.y*(1-playerData.jumpCutMultiplier), ForceMode2D.Impulse);
           coyoteTimeCounter = 0f;
        }
    }

    #endregion

    void Update()
    {
        
        if (playerData.gravityMode == GravityMode.Up){
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (playerData.gravityMode == GravityMode.Left){
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (playerData.gravityMode == GravityMode.Right){
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else{
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        

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
            rb.AddForce(Vector2.up * playerData.jumpForce * playerData.jumpMod, ForceMode2D.Impulse);
            
            jumpBufferCounter = 0f;
        }

        #region Animation States
        if(Mathf.Abs(moveInput) > 0f){
            playerData.running = true;
        }
        else{
            playerData.running = false;
        }
        if (Mathf.Abs(playerData.velocity.y) == 0f || playerData.grounded)
        {
            playerData.falling = false;
            playerData.jumping = false;
        }
        else if (playerData.velocity.y > 0f){
            playerData.jumping = true;
            playerData.falling = false;
        }
        else{
            playerData.jumping = false;
            playerData.falling = true;
        }
        #endregion

        //comprobar si hay que girar el sprite
        if ((playerData.gravityMode != GravityMode.Up && !playerData.isFacingRight && moveInput > 0f && !playerData.midAttacking && !playerData.pushing) ||
                (playerData.gravityMode == GravityMode.Up && playerData.isFacingRight && moveInput > 0f && !playerData.midAttacking && !playerData.pushing))
        {
            Flip();
        }
        else if ((playerData.gravityMode != GravityMode.Up && playerData.isFacingRight && moveInput < 0f && !playerData.midAttacking && !playerData.pushing)
                || (playerData.gravityMode == GravityMode.Up && !playerData.isFacingRight && moveInput < 0f && !playerData.midAttacking && !playerData.pushing))
        {
            Flip();
        }


        #region debug
        groundPosition = playerData.groundCheck.position;
        groundRadius = playerData.groundCheckRadius;
        #endregion
        
    }

    public void ChangeXMovement(float input)
    {
        if (!playerData.movingWithPowers && !playerData.attacking){
            #region Run
            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed;
            if (playerData.wallWalking)
            {
                targetSpeed = input * playerData.moveSpeed * playerData.crocuhModifier * playerData.moveMod;
            }
            else
            {
                targetSpeed = input * playerData.moveSpeed * playerData.moveMod;
            }
            //calculate the difference between our current speed and the target speed
            float speedDif = targetSpeed - playerData.velocity.x;
            //change the speed based on the acceleration or decceleration rate
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.acceleration : playerData.decceleration;
            //applies acceleration to speed difference, the raises to a set power so accelerration increases with higher speeds.
            //finally multiplies by a sign to reapply direction
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, playerData.velPower) * Mathf.Sign(speedDif);
            
            rb.AddForce(movement * RunDirectionByGravity(playerData.gravityMode));
                
            #endregion
        }
    }

    void FixedUpdate()
    {
        ChangeXMovement(moveInput);
        
        #region Friction

        if (Mathf.Abs(moveInput) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(playerData.velocity.x), Mathf.Abs(playerData.frictionAmount));

            amount *= Mathf.Sign(playerData.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        #endregion

        FallGravityController();
        assignVelocity();

    }
    private void assignVelocity(){
        if (playerData.gravityMode == GravityMode.Up){
            playerData.velocity.x = rb.velocity.x;
            playerData.velocity.y = rb.velocity.y *-1f;
        }
        else if (playerData.gravityMode == GravityMode.Left){
            playerData.velocity.x = rb.velocity.y *-1f;
            playerData.velocity.y = rb.velocity.x *-1f;
        }
        else if (playerData.gravityMode == GravityMode.Right){
            playerData.velocity.x = rb.velocity.y;
            playerData.velocity.y = rb.velocity.x;
        }
        else{
            playerData.velocity.x = rb.velocity.x;
            playerData.velocity.y = rb.velocity.y;
        }
    }

    private Vector2 RunDirectionByGravity(GravityMode gravity){
        if(gravity == GravityMode.Down || gravity == GravityMode.Up){
            return Vector2.right;
        }
        else if (gravity == GravityMode.Left){
             return Vector2.down;
        }
        else if (gravity == GravityMode.Right){
             return Vector2.up;
        }
        return Vector2.zero;
    }

    private void FallGravityController(){
        if (playerData.gravityMode == GravityMode.Down){
            if ((playerData.jumping || playerData.falling) && Mathf.Abs(playerData.velocity.y) < playerData.jumpHangTheshold){
                playerData.setGravity(0f, playerData.jumpHangMultiplier);
            }  
            else if (playerData.velocity.y < 0f)
            {
                playerData.setGravity(0f, playerData.fallGravityMultiplier);
            }
            else{
                playerData.setGravity(0f,1f);
            }
        }
    }


    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    private void Flip()
    {
        if (Time.timeScale == 1f)
        {
            playerData.isFacingRight = !playerData.isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            /*
            Vector3 currentShootPoint = playerData.shootPoint.localPosition;
            currentShootPoint.x *= -1f;
            playerData.shootPoint.localPosition = currentShootPoint;
            */
        }
    }

    private void OnDrawGizmos()
    {
        if(active){
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundPosition,groundRadius);
            /*
            Vector2 positon = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = rb.velocity+positon;
            Gizmos.DrawLine(transform.position, direction);
            */

        }
    }


}