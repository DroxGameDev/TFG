using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    PlayerData playerData;
    Rigidbody2D rb;
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
    }

    public void OnDamage(int amount, float knockbackAmount, bool originFacingRight)
    {
        if (invincibilityTimer <= 0f)
        {
            Vector2 knockbackDirection;
            rb.velocity = Vector2.zero;

            if (originFacingRight)
            {
                if (playerData.isFacingRight)
                {
                    playerData.Flip();
                }
                knockbackDirection = Vector2.right;
            }
            else
            {
                if (!playerData.isFacingRight)
                {
                    playerData.Flip();
                }
                knockbackDirection = Vector2.left;
            }

            playerData.health -= amount;
            if (playerData.health <= 0)
                OnDie();

            HitStop.Instance.Stop(playerData.hitTime);
            StartCoroutine(DamageRutine(knockbackDirection, knockbackAmount));
        }
    }

    IEnumerator DamageRutine(Vector2 direction, float knockback)
    {
        playerData.damaged = true;

        yield return new WaitForSeconds(playerData.damageWait);

        playerData.damaged = false;

        invincibilityTimer = playerData.invicibilityTime;

        Color colorA = Color.white;
        Color colorB = Color.black;

        float lerpSpeed = playerData.invicibilityEffectSpeed; // Puedes ajustar esto para que el parpadeo sea más o menos rápido
        float t = 0f;
        bool fadingToBlack = true;

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
        Debug.Log("Die");
    }
}
