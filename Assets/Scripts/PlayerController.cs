using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput playerControls;

    void Sart()
    {
         playerControls = new PlayerInput();
         rb = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>().x);
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
