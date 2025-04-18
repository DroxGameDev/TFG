using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExportedPlayerController : MonoBehaviour
{
    /////////////////////////////////
    /// <summary>
    /// Movimiento 2D de Scroll lateral con:
    /// �Input system
    /// �Altura de salto dependiente del tiempo pulsado
    /// �Coyote Time
    /// �Jump Buffering
    /// �Correcci�n de Esquinas
    /// �Giro del personaje dependiendo de la direcci�n
    /// </summary>
    ///////////////////////////////////



    private Rigidbody2D rb;

    // variables para comprobar si esta en el suelo
    public Transform groundCheck;
    public LayerMask groundLayer;

    //variables movimiento 
    public float moveSpeed;
    public float jumpPower;

    //valor que devuelve el Input
    private float moveInput;

    //variables del CoyoteTime (que pueda saltar un poquito despues de salirse de la plataforma)
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    //variables del JumpBuffering (que pueda saltar un poco antes de llegar al suelo cuando cae)
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    //para comprobar la direccion en la que mire en sprite (por defecto tiene que estar mirando a la derecha)
    private bool isFacingRight = true;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);

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

    // Update de las fisicas de movimiento
    void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    //funci�n que se activa cuando se pulsa el input de saltar (input system)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    //devuelve si el personaje esta en el suelo
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    //funci�n que se activa cuando se pulsa el input de moverse (input system)
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }

    // funci�n para girar al personaje segun la direcci�n que mire
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}
