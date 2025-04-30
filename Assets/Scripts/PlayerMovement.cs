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

    [Space(10)]

    [Header("Steel")]
    //Steel = push
    private List<LineObject> nearMetalLines;
    private Vector2 selectMetalVector;
    private Vector2 selecteMetalThreshold;
    private LineObject selectedMetal = null;

    private float steelPushCounter;

    [Space(10)] 


    private float lastGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();

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

    public IEnumerator SteelInputupdate(bool context)
    {
        if (context && nearMetalLines.Count == 0){

            playerData.timeStoped = true;

            Collider2D[] nearMetals = Physics2D.OverlapCircleAll(transform.position, playerData.metalCheckRadius, playerData.metalEnvironmentLayer);

            for (int i = 0; i < nearMetals.Length; i++){

                GameObject newLinePrefab = Instantiate(playerData.linePrefab);
                LineObject newLineObject = new LineObject(newLinePrefab, nearMetals[i]);
                nearMetalLines.Add(newLineObject);
            }    

            #region metal lines positioning
            for(int i = 0; i < nearMetalLines.Count; i++){
                LineObject actualLine = nearMetalLines[i];
                
                actualLine.lineRenderer.SetPosition(0, transform.position);
                actualLine.lineRenderer.SetPosition(1, actualLine.metal.transform.position);

                Vector2 MetalClosestPoint = nearMetalLines[i].metal.GetComponent<BoxCollider2D>().ClosestPoint(transform.position);
                float lineDistance = Vector2.Distance(transform.position, MetalClosestPoint);

                if(lineDistance <= playerData.metalCheckMinRadius){
                    if (actualLine.iValue != 1){
                        actualLine.iValue = 1f;
                        ChangeMaterialAlpha(actualLine.lineRenderer.material, 1f);
                    }  
                }
                else{
                        actualLine.iValue = Mathf.InverseLerp(playerData.metalCheckRadius,playerData.metalCheckMinRadius, lineDistance);

                        if (actualLine.iValue > 0.5f){
                            actualLine. iValue = 0.5f;
                        }
                        else if (actualLine.iValue >= 0.01){
                            actualLine.iValue = 0.1f;
                        }

                        ChangeMaterialAlpha(actualLine.lineRenderer.material, actualLine.iValue);        
                }

                actualLine.lineRenderer.material.SetFloat("_GlowAmount", 0);
            }
            #endregion

        }

        Time.timeScale = 0;

        while(context)
            yield return null;

        playerData.timeStoped = false;
        
        if (!context && nearMetalLines.Count > 0){

            steelPushCounter = playerData.steelPushTime;
            playerData.burningSteel = true;

            if(selectedMetal != null){

                Vector2 directionVector = selectedMetal.metal.transform.position-transform.position;
                directionVector.Normalize();
                rb.velocity = Vector3.zero;
                rb.AddForce(directionVector * playerData.steelPushPower * selectedMetal.iValue * -1, ForceMode2D.Impulse);
            }

            selectedMetal = null;
            for (int i = 0; i < nearMetalLines.Count; i++){
                Destroy(nearMetalLines[i].line);
            }    
            nearMetalLines.Clear();
        }
        Time.timeScale = 1;
    }

    public void GetSelectMetalAngle(Vector2 context)
    {
        selectMetalVector = context;
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

        if(steelPushCounter>0){
            steelPushCounter -= Time.deltaTime;
        }
        else{
            playerData.burningSteel = false;
            steelPushCounter = 0f;
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
        if (!isFacingRight && moveInput > 0f && !playerData.burningSteel)
        {
            Flip();
        }
        else if (isFacingRight && moveInput < 0f && !playerData.burningSteel)
        {
            Flip();
        }

        //actualizar lineas
        if(nearMetalLines.Count > 0){
            
            float selectedMetalAngle = 360f;

            for(int i = 0; i< nearMetalLines.Count; i++){
                Vector2 actualMetalVector = nearMetalLines[i].metal.transform.position-transform.position;
                float posibleAngle = Vector2.Angle(selectMetalVector, actualMetalVector);

                if (posibleAngle < selectedMetalAngle){
                    selectedMetal = nearMetalLines[i];
                    selectedMetalAngle = posibleAngle;
                }

                nearMetalLines[i].lineRenderer.material.SetFloat("_GlowAmount", 0);
            }

            selectedMetal.lineRenderer.material.SetFloat("_GlowAmount", 1);
            
        }
        
    }

    void FixedUpdate()
    {
        if (!playerData.burningSteel){
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
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer) ||
                Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.metalEnvironmentLayer);
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

    private void ChangeMaterialAlpha(Material material, float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }
}