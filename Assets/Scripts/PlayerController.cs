using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerControls;
    private PlayerMovement playerMovement;
    private PlayerAnimations playerAnimations;

    void Start()
    {
        playerControls = new PlayerInput();
        playerMovement = GetComponent<PlayerMovement>();    
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovement.moveInputUpdate(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context){
        if (context.performed)
        {
            Debug.Log("Jump performed");
        }

        if (context.canceled /*&& rb.velocity.y > 0f*/)
        {
            Debug.Log("Jump canceled");
        }
    }
}
