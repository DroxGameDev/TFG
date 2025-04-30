using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerInput playerControls;
    private PlayerMovement playerMovement;
    private PlayerAnimations playerAnimations;
    
    private Rigidbody2D rb;

    private Vector2 i;
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();    
        playerAnimations = GetComponent<PlayerAnimations>();

        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovement.moveInputUpdate(context.ReadValue<Vector2>());

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

    public void SteelPush(InputAction.CallbackContext context){
        if (context.performed)
           StartCoroutine(playerMovement.SteelInputupdate(true));

        if (context.canceled)
           StartCoroutine(playerMovement.SteelInputupdate(false));
    }

    public void SelectMetal(InputAction.CallbackContext context){
        playerMovement.GetSelectMetalAngle(context.ReadValue<Vector2>());
        i = context.ReadValue<Vector2>();
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(i.x, i.y, 0));
    }
    
    
}
