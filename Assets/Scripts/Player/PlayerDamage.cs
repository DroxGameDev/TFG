using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    PlayerData playerData;
    Rigidbody2D rb;
    PlayerDie playerDie;
    PlayerInput playerInput;
    private float invincibilityTimer = 0f;

    public int Health
    {
        get => playerData.health;
        set => playerData.health = value;
    }
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        playerDie = GetComponent<PlayerDie>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnDamage(int amount, float knockbackAmount, bool originFacingRight)
    {
        if (invincibilityTimer <= 0f)
        {
            Vector2 knockbackDirection;
            playerData.velocity = Vector2.zero;

            if (originFacingRight)
            {
                if (playerData.isFacingRight)
                {
                    playerData.Flip();
                }
                knockbackDirection = new Vector2(1, 1);
                //knockbackDirection = Vector2.right;
            }
            else
            {
                if (!playerData.isFacingRight)
                {
                    playerData.Flip();
                }
                //knockbackDirection = Vector2.left;
                knockbackDirection = new Vector2(-1,1);
            }

            playerData.health -= amount;
            if (playerData.health <= 0)
                OnDie();
            else
            {
                HitStop.Instance.Stop(playerData.hitTime);
                StartCoroutine(DamageWait());
                StartCoroutine(DamageFeedback(knockbackDirection, knockbackAmount));
            }
        }
    }
    IEnumerator DamageWait()
    {
        rb.velocity = Vector2.zero;
        playerData.damaged = true;
        playerInput.actions.Disable();

        yield return new WaitForSeconds(playerData.damageWait);

        playerData.damaged = false;
        playerInput.actions.Enable();
    }
    IEnumerator DamageFeedback(Vector2 direction, float knockback)
    {
        invincibilityTimer = playerData.invicibilityTime;

        Color colorA = Color.white;
        Color colorB = Color.black;

        float lerpSpeed = playerData.invicibilityEffectSpeed; // Puedes ajustar esto para que el parpadeo sea más o menos rápido
        float t = 0f;
        bool fadingToBlack = true;
        
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);

        while (invincibilityTimer > 0f)
        {
            if (fadingToBlack)
            {
                t += Time.deltaTime * lerpSpeed;
                if (t >= 1f)
                {
                    t = 1f;
                    fadingToBlack = false;
                }
            }
            else
            {
                t -= Time.deltaTime * lerpSpeed;
                if (t <= 0f)
                {
                    t = 0f;
                    fadingToBlack = true;
                }
            }

            playerData.sprite.color = Color.Lerp(colorA, colorB, t);

            invincibilityTimer -= Time.deltaTime;
            yield return null;
        }
        // Restaurar color original al final
        playerData.sprite.color = Color.white;
    }
    
    public void OnDie()
    {
        playerDie.Die();
    }
}
