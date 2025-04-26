using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerControls;
    private PlayerMovement playerMovement;
    private PlayerAnimations playerAnimations;
    
    private Rigidbody2D rb;

    private bool moveInput = false;

    void Start()
    {
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();    
        playerAnimations = GetComponent<PlayerAnimations>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovement.moveInputUpdate(context.ReadValue<Vector2>());

        if(Mathf.Abs(context.ReadValue<Vector2>().x) > 0)   moveInput = true;        
        else  moveInput = false;
    }

    public void OnJump(InputAction.CallbackContext context){
        if (context.performed)
        {
           playerMovement.jumpInputUpdate(true);
        }

        if (context.canceled)
        {
           playerMovement.jumpInputUpdate(false);
        }
    }

    void Update()
    {
        #region Animation States
        if (playerMovement.IsGrounded())
        {
            playerAnimations._grounded = true;
        }
        else
        {
            playerAnimations._grounded = false;
        }
       
        if(moveInput && Mathf.Abs(rb.velocity.x) > 0){
            playerAnimations._running = true;
        }
        else{
            playerAnimations._running = false;
        }

        if (Mathf.Abs(rb.velocity.y) == 0f)
        {
            playerAnimations._falling = false;
            playerAnimations._jumping = false;
        }
        else if (rb.velocity.y > 0f){
            playerAnimations._jumping = true;
        }
        else{
            playerAnimations._jumping = false;
            playerAnimations._falling = true;
        }
        #endregion

    }
}
