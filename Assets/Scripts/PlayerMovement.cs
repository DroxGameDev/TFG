using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private float moveInput;
    private bool isFacingRight = true;

    [Range (0f, 10f)] public float moveSpeed = 5f;
    [Range (0f, 10f)] public float acceleration = 5f;
    [Range (0f, 10f)] public float decceleration = 5f;
    [Range (0f, 10f)] public float velPower = 5f;

     

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void moveInputUpdate(Vector2 context)
    {
        moveInput = context.x;
    }

    void Update()
    {
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
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 1f) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
        #endregion
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector2 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}