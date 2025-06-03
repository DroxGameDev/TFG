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
    private SteelPower steelPower;
    private IronPower ironPower;
    private TinPower tinPower;
    private PewterPower pewterPower;
    //private Rigidbody2D rb;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerResources = GetComponent<PlayerResources>();
        playerControls = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        steelPower = GetComponent<SteelPower>();
        ironPower = GetComponent<IronPower>();
        tinPower = GetComponent<TinPower>();
        pewterPower = GetComponent<PewterPower>();

        //rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        playerMovement.MoveInputUpdate(context.ReadValue<Vector2>());

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerData.attacking && !playerData.pushing)
        {
            if (context.performed)
            {
                playerMovement.JumpInputUpdate(true);
            }

            if (context.canceled)
            {
                playerMovement.JumpInputUpdate(false);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!playerData.wallWalking && !playerData.burningIron && !playerData.burningSteel && !playerData.showingCoin)
            if (context.performed) playerAttack.OnAttack();
    }

    public void OnPush(InputAction.CallbackContext context)
    {
        if (context.performed && playerData.objectNearby && playerData.burningPewter) pewterPower.PushImputUpdate();
    }

    public void SteelPush(InputAction.CallbackContext context)
    {
        if (!playerData.attacking && !playerResources.steelEmpty)
        {
            if (context.performed)
                StartCoroutine(steelPower.SteelInputupdate(true));

            if (context.canceled)
                StartCoroutine(steelPower.SteelInputupdate(false));
        }
    }

    public void IronPull(InputAction.CallbackContext context)
    {
        if (!playerData.attacking && !playerResources.ironEmpty)
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
            Iron_Steel.GetSelectMetalAngle(context.ReadValue<Vector2>());
            return;
        }

        Vector2 playerinScreen = playerData.mainCamera.WorldToScreenPoint(transform.position);
        Vector2 direction = (context.ReadValue<Vector2>() - playerinScreen).normalized;

        Iron_Steel.GetSelectMetalAngle(direction);

    }

    public void PickDropCoin(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed) playerResources.PickDropCoinInput();
        }
    }

    public void ShowCoin(InputAction.CallbackContext context)
    {
        if (!playerData.attacking)
        {
            if (context.performed)
            {
                playerResources.ShowCoinInput();
            }
        } 
    }

    public void TinSenses(InputAction.CallbackContext context){
        if (context.performed && !playerResources.tinEmpty) tinPower.TinInput();
    }

    public void PewterEnhance(InputAction.CallbackContext context)
    {
        if (context.performed && !playerResources.pewterEmpty) pewterPower.PewterInput();
    }
}
