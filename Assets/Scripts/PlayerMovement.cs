using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D rb;
    

    [Header("Movement")]
    [Range (0f, 20f)] public  float moveSpeed = 5f;
    [Range (0f, 30f)] public  float acceleration = 5f;
    [Range (0f, 30f)] public  float decceleration = 24f;
    [Range (0f, 1.5f)] public  float velPower = 0.9f;

    [Space(10)] 
    [Range (0f, 0.5f)] public  float frictionAmount = 0.2f;

    private float moveInput;
    private bool isFacingRight = true;

    [Header("Jumping")]
    [Range (0f, 20f)] public  float jumpForce = 15f;
    [Range (0f, 1f)] public  float jumpCutMultiplier = 0.5f;
    [Space(10)] 
    [Range (0f, 1f)] public  float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [Range (0f, 1f)] public  float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Space(10)] 
    [Range (0f, 5f)] public  float gravityScale = 1f;
    [Range (0f, 5f)] public  float fallGravityMultiplier = 2f;
    
    [Range (0f, 5f)] public float jumpHangTheshold = 0.5f;
    [Range (0f, 5f)] public float jumpHangMultiplier = 0.5f;


    [Space(10)]

    [Header("Check")]
    public Transform groundCheck;
    [Range (0f, 0.5f)] public  float groundCheckRadius = 0.1f;
    
    [Space(10)]

    public LayerMask groundLayer;

    [Header("Steel and Iron")]
    //Steel = push
    //Iron = Iron
    [SerializeField] private LayerMask metalEnviorimentLayer;
    [SerializeField, Range (0f, 10f)] private float metalCheckRadius;
    private bool SteelInput;
    private Collider2D[] nearMetals;
    private List<GameObject> nearMetalLines;

    public GameObject linePrefab;

    [Space(10)] 


    private float lastGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nearMetalLines = new List<GameObject>();
    }

    #region Input
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

    public void SteelInputupdate(bool context)
    {
        if (context){
            nearMetals = Physics2D.OverlapCircleAll(transform.position, metalCheckRadius, metalEnviorimentLayer);

            for (int i = 0; i < nearMetals.Length; i++){
                GameObject newLinePrefab = Instantiate(linePrefab);
                nearMetalLines.Add(newLinePrefab);
            }    
        }

        if (!context){
            for (int i = 0; i < nearMetalLines.Count; i++){
                Destroy(nearMetalLines[i]);
            }    
            nearMetalLines.Clear();
        }
    }
    #endregion

    void Update()
    {
        
         //Raycasts para la correcci�n de esquinas
        
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        jumpBufferCounter -= Time.deltaTime;

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
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

        //actualizar lineas
        if(nearMetalLines.Count > 0){
            for(int i = 0; i < nearMetalLines.Count; i++){

                nearMetalLines[i].GetComponent<LineRenderer>().SetPosition(0, transform.position);
                nearMetalLines[i].GetComponent<LineRenderer>().SetPosition(1, nearMetals[i].transform.position);
            }
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

    }

    public void GravityController(bool isFalling, bool isJumping){
        if ((isJumping || isFalling) && Mathf.Abs(rb.velocity.y) < jumpHangTheshold){
            setGravityScale(gravityScale*jumpHangMultiplier);
        }  
        else if (rb.velocity.y < 0f)
        {
            setGravityScale(gravityScale* fallGravityMultiplier);
        }
        else{
            setGravityScale(gravityScale);
        }
    }

    private void setGravityScale(float newGravity){
        rb.gravityScale = newGravity;
    }

    public bool IsGrounded()
    {
        return (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) || 
                Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, metalEnviorimentLayer));
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}