using System;
using System.Collections;
using System.Collections.Generic;
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

    [Space(10)]

    [Header("Steel")]
    //Steel = push

    //private bool SteelInput;
    private Collider2D[] nearMetals;
    private List<LineObject> nearMetalLines;

    private float selectedMetalAngle = -1;

    [Space(10)] 


    private float lastGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();
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

    public void SteelInputupdate(bool context)
    {
        if (context && nearMetalLines.Count == 0){
            nearMetals = Physics2D.OverlapCircleAll(transform.position, playerData.metalCheckRadius, playerData.metalEnvironmentLayer);

            for (int i = 0; i < nearMetals.Length; i++){

                GameObject newLinePrefab = Instantiate(playerData.linePrefab);
                LineObject newLineObject = new LineObject(newLinePrefab, nearMetals[i]);
                nearMetalLines.Add(newLineObject);
            }    
        }

        if (!context && nearMetalLines.Count > 0){
            for (int i = 0; i < nearMetalLines.Count; i++){
                Destroy(nearMetalLines[i].line);
            }    
            nearMetalLines.Clear();
        }
    }

    public void GetSelectMetalAngle(Vector2 context){
        if (context.x == 0 && context.y == 0){
            selectedMetalAngle = 360f;
        }
        else{
            selectedMetalAngle = Mathf.Round(Vector2.SignedAngle(Vector2.right, context)*100f)*0.01f;         
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
        if(Mathf.Abs(moveInput) > 0f && Mathf.Abs(rb.velocity.x) > 0){
            playerData.running = true;
        }
        else{
            playerData.running = false;
        }

        if (Mathf.Abs(rb.velocity.y) == 0f)
        {
            playerData.falling = false;
            playerData.jumping = false;
        }
        else if (rb.velocity.y > 0f){
            playerData.jumping = true;
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

        //actualizar lineas
        if(nearMetalLines.Count > 0){
            for(int i = 0; i < nearMetalLines.Count; i++){
                LineObject actualLine = nearMetalLines[i];
                
                actualLine.lineRenderer.SetPosition(0, transform.position);
                actualLine.lineRenderer.SetPosition(1, nearMetalLines[i].metal.transform.position);

                Vector2 MetalClosestPoint = nearMetalLines[i].metal.GetComponent<BoxCollider2D>().ClosestPoint(transform.position);
                float lineDistance = Vector2.Distance(transform.position, MetalClosestPoint);

                if(Vector2.Distance(transform.position, MetalClosestPoint) <= playerData.metalCheckMinRadius){
                    if (actualLine.iValue != 1){
                        actualLine.iValue = 1f;
                        ChangeMaterialAlpha(actualLine.lineRenderer.material, 1f);
                    }  
                }
                else{
                    if (actualLine.iValue == 1 || actualLine.iValue == 0){
                        actualLine.iValue = Mathf.InverseLerp(playerData.metalCheckRadius,playerData.metalCheckMinRadius, lineDistance);

                        if (actualLine.iValue > 0.5f){
                            actualLine. iValue = 0.5f;
                        }
                        else if (actualLine.iValue >= 0.01){
                            actualLine.iValue = 0.1f;
                        }

                        ChangeMaterialAlpha(actualLine.lineRenderer.material, actualLine.iValue);   
                          
                    }        
                }

                actualLine.angle = Mathf.Round(Vector2.SignedAngle(Vector2.right,nearMetalLines[i].metal.transform.position-transform.position)* 100f) * 0.01f;
                //Debug.Log(actualLine.angle);
            }
            nearMetalLines.Sort((left, right) => left.angle.CompareTo(right.angle));

            /*
            int nearestMetal = -1;
            float nearestDistance = 180;
            for (int i = 0; i <= nearMetalLines.Count; i++){
                
            }
            */
        }
        
    }

    void FixedUpdate()
    {
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
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer) ||
                Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.metalEnvironmentLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void ChangeMaterialAlpha(Material material, float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,metalCheckRadius);
    }
    */
}