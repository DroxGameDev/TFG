using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerMovement : AffectedByGravity
{
    private PlayerData playerData;
    

    [Header("Movement")]
    private float moveInput;

    [Header("Jumping")]
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Debug")]
    public bool active;

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

                playerData.velocity = Vector2.zero;

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

    public void InteractInput()
    {
        GameManager.Instance.PlayerToDoor(playerData.nearbyDoor);
    }

    #endregion

    void Update()
    {

        if (playerData.gravityMode == GravityMode.Up)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (playerData.gravityMode == GravityMode.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (playerData.gravityMode == GravityMode.Right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
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

        if(CheckFlip(moveInput)) playerData.Flip();
        
    }

    private bool CheckFlip(float moveInput)
    {
        if (playerData.attacking || playerData.pushing || playerData.damaged || playerData.movingWithPowers)
            return false;

        bool movingRight = moveInput > 0f;
        bool movingLeft = moveInput < 0f;

        if (movingRight)
        {
            return (playerData.gravityMode != GravityMode.Up && !playerData.isFacingRight) ||
                (playerData.gravityMode == GravityMode.Up && playerData.isFacingRight);
        }
        else if (movingLeft)
        {
            return (playerData.gravityMode != GravityMode.Up && playerData.isFacingRight) ||
                (playerData.gravityMode == GravityMode.Up && !playerData.isFacingRight);
        }

        return false;
    }

    private void ChangeXMovement(float input)
    {
        if (playerData.preparingAttack || playerData.attacking)
            input = 0f;

        if (playerData.movingWithPowers || (!playerData.burningPewter && playerData.pushing) || playerData.damaged) return;

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

            if (Mathf.Abs(targetSpeed) > 0f && !playerData.grounded)
            {
                if (targetSpeed > 0f && targetSpeed < playerData.velocity.x)
                {
                    targetSpeed = playerData.velocity.x;
                }
                else if (targetSpeed < 0f && targetSpeed > playerData.velocity.x)
                {
                    targetSpeed = playerData.velocity.x;
                }
            }
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

    private Vector2 RunDirectionByGravity(GravityMode gravity)
    {
        if (gravity == GravityMode.Down || gravity == GravityMode.Up)
        {
            return Vector2.right;
        }
        else if (gravity == GravityMode.Left)
        {
            return Vector2.down;
        }
        else if (gravity == GravityMode.Right)
        {
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
        /*
        Vector2 a = new Vector2(playerData.groundCheck.position.x + playerData.groundCheckVertexA_x, playerData.groundCheck.position.y + playerData.groundCheckVertexA_y);
        Vector2 b = new Vector2(playerData.groundCheck.position.x + playerData.groundCheckVertexB_x, playerData.groundCheck.position.y + playerData.groundCheckVertexB_y);

        return Physics2D.OverlapArea(a, b, playerData.groundLayer);
        */

        Bounds bounds = GetComponent<BoxCollider2D>().bounds;

        // Tres puntos: centro, izquierda y derecha
        Vector2 left = new Vector2(bounds.min.x + 0.05f, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - 0.05f, bounds.min.y);

        // Raycasts hacia abajo
        bool isGrounded =
            Physics2D.Raycast(left, Vector2.down, playerData.groundCheckDistance, playerData.groundLayer) ||
            Physics2D.Raycast(center, Vector2.down, playerData.groundCheckDistance, playerData.groundLayer) ||
            Physics2D.Raycast(right, Vector2.down, playerData.groundCheckDistance, playerData.groundLayer);

        // (Opcional) Dibuja los rayos en la vista de escena
        /*
        Debug.DrawRay(left, Vector2.down * playerData.groundCheckDistance, Color.red);
        Debug.DrawRay(center, Vector2.down * playerData.groundCheckDistance, Color.red);
        Debug.DrawRay(right, Vector2.down * playerData.groundCheckDistance, Color.red);
        */
        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        if(active){

        }
    }


}