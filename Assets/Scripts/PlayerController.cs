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
    private SteelPower steelPower;
    
    private Rigidbody2D rb;

    private Vector2 i;
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();    
        playerAnimations = GetComponent<PlayerAnimations>();
        steelPower = GetComponent<SteelPower>();

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
           StartCoroutine(steelPower.SteelInputupdate(true));

        if (context.canceled)
           StartCoroutine(steelPower.SteelInputupdate(false));
    }

    public void SelectMetal(InputAction.CallbackContext context){
        steelPower.GetSelectMetalAngle(context.ReadValue<Vector2>());
        i = context.ReadValue<Vector2>();
    }

    /*  
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,playerData.metalCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,playerData.metalCheckMinRadius);

    }
    */
}
