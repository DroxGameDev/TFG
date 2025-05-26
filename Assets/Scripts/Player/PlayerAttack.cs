using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
public enum AttackCombo
{
    Attack1,
    Attack2,
    Attack3
}

public class PlayerAttack : MonoBehaviour
{
    private PlayerData playerData;
    private Rigidbody2D rb;
    private float attackBufferCounter;
    private float attackCooldownCounter;
    private float attackComboCounter;
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        playerData.attackCollider.enabled = false;
    }
    public void OnAttack()
    {
        attackBufferCounter = playerData.attackBufferTime;
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(playerData.waitForAttack);

        playerData.attackCollider.enabled = true;
        playerData.midAttacking = true;

        if (playerData.isFacingRight)
        {
            rb.AddForce(Vector2.right * playerData.attackImpulse, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.left * playerData.attackImpulse, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(playerData.attackCooldownTime - playerData.waitForAttack);
        playerData.attackCollider.enabled = false;
        playerData.midAttacking = false;

    }

    void Update()
    {
        attackComboCounter -= Time.deltaTime;
        attackCooldownCounter -= Time.deltaTime;
        attackBufferCounter -= Time.deltaTime;

        if (attackCooldownCounter < 0.01f && attackBufferCounter > 0.01f)
        {
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * playerData.velocity.y*(1-playerData.jumpCutMultiplier), ForceMode2D.Impulse);
            }
            rb.velocity = new Vector2(0f, rb.velocity.y);
            attackCooldownCounter = playerData.attackCooldownTime;
            playerData.attacking = true;

            if (attackComboCounter > 0.01f)
            {
                switch (playerData.attackComboStep)
                {
                    case AttackCombo.Attack1:
                        playerData.attackComboStep = AttackCombo.Attack2;
                        StartCoroutine(Attack());
                        break;
                    case AttackCombo.Attack2:
                        playerData.attackComboStep = AttackCombo.Attack3;
                        StartCoroutine(Attack());
                        break;
                    default:
                        playerData.attackComboStep = AttackCombo.Attack1;
                        StartCoroutine(Attack());
                        break;
                }
            }
            else
            {
                playerData.attackComboStep = AttackCombo.Attack1;
                StartCoroutine(Attack());
            }

            attackComboCounter = playerData.attackComboTime;
        }

        if (attackCooldownCounter < 0.01f)
        {
            playerData.attacking = false;
        }
    }
}
