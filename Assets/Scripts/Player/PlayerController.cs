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
    private IronPower ironPower;
    private Rigidbody2D rb;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();    
        playerAnimations = GetComponent<PlayerAnimations>();
        steelPower = GetComponent<SteelPower>();
        ironPower = GetComponent<IronPower>();

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

    public void IronPull(InputAction.CallbackContext context){
        if (context.performed)
           StartCoroutine(ironPower.IronInputupdate(true));

        if (context.canceled)
           StartCoroutine(ironPower.IronInputupdate(false));
    }

    public void SelectMetal(InputAction.CallbackContext context){
        if(playerControls.currentControlScheme == "GamePad"){
            Iron_Steel.GetSelectMetalAngle(context.ReadValue<Vector2>());
            return;
        }

        Vector2 playerinScreen = playerData.mainCamera.WorldToScreenPoint(transform.position);
        Vector2 direction = (context.ReadValue<Vector2>() - playerinScreen).normalized;

        Iron_Steel.GetSelectMetalAngle(direction);
        
    }
}
