using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDie : MonoBehaviour
{

    private PlayerInput playerControls;
    private PlayerData playerData;
    private Rigidbody2D rb;

    void Start()
    {
        playerControls = GetComponent<PlayerInput>();
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        playerData.dead = true;
        playerControls.actions.Disable();
        Time.timeScale = 0.5f;
        playerData.damageCollider.enabled = false;

        rb.velocity = Vector2.zero;
        float knockbackDirection = playerData.isFacingRight ? -1 : 1;
        Vector2 knockBackForce = new Vector2(playerData.deathKnockBack * knockbackDirection, playerData.deathKnockBack);

        rb.AddForce(knockBackForce, ForceMode2D.Impulse);
        GameManager.Instance.RespawnPlayer();
    }

    public void RespawnValues()
    {
        playerData.dead = false;
        playerControls.actions.Enable();
        playerData.damageCollider.enabled = true;
    }

}
