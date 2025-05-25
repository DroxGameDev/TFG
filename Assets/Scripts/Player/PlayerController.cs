using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerResources playerResources;
    private PlayerInput playerControls;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private PlayerAnimations playerAnimations;
    private SteelPower2 steelPower;
    private IronPower2 ironPower;
    private TinPower tinPower;
    private Rigidbody2D rb;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerResources = GetComponent<PlayerResources>();
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        playerAnimations = GetComponent<PlayerAnimations>();
        steelPower = GetComponent<SteelPower2>();
        ironPower = GetComponent<IronPower2>();
        tinPower = GetComponent<TinPower>();

        rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovement.moveInputUpdate(context.ReadValue<Vector2>());

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed)
            {
                playerMovement.jumpInputUpdate(true);
            }

            if (context.canceled)
            {
                playerMovement.jumpInputUpdate(false);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!playerData.wallWalking && !playerData.timeStoped && !playerData.movingWithPowers && !playerData.showingCoin)
            if (context.performed) playerAttack.OnAttack();
    }

    public void SteelPush(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed)
                StartCoroutine(steelPower.SteelInputupdate(true));

            if (context.canceled)
                StartCoroutine(steelPower.SteelInputupdate(false));
        }
    }

    public void IronPull(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed)
                StartCoroutine(ironPower.IronInputupdate(true));

            if (context.canceled)
                StartCoroutine(ironPower.IronInputupdate(false));
        }
    }

    public void SelectMetal(InputAction.CallbackContext context)
    {
        if (playerControls.currentControlScheme == "GamePad")
        {
            Iron_Steel2.GetSelectMetalAngle(context.ReadValue<Vector2>());
            return;
        }

        Vector2 playerinScreen = playerData.mainCamera.WorldToScreenPoint(transform.position);
        Vector2 direction = (context.ReadValue<Vector2>() - playerinScreen).normalized;

        Iron_Steel2.GetSelectMetalAngle(direction);

    }

    public void PickDropCoin(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed) playerResources.PickDropCoin();
        }
    }

    public void ShowCoin(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed)
            {
                playerResources.ShowCoin();
            }
        } 
    }

    public void TinSenses(InputAction.CallbackContext context){
        if (context.performed) tinPower.TinInput();
    }
}
