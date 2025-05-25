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
    }
    public void OnAttack()
    {
        attackBufferCounter = playerData.attackBufferTime;
    }

    private IEnumerator AttackImpulse()
    {
        yield return new WaitForSeconds(playerData.waitForAttackImpulse);

        if (playerData.isFacingRight)
        {
            if (playerData.attackComboStep == AttackCombo.Attack1 || playerData.attackComboStep == AttackCombo.Attack3)
            {
                rb.AddForce(Vector2.right * playerData.attackImpulse, ForceMode2D.Impulse);
            }
            else if (playerData.attackComboStep == AttackCombo.Attack2)
            {
                rb.AddForce(Vector2.left * playerData.attackImpulse, ForceMode2D.Impulse);
            }
        }
        else
        {
            if (playerData.attackComboStep == AttackCombo.Attack1 || playerData.attackComboStep == AttackCombo.Attack3)
            {
                rb.AddForce(Vector2.left * playerData.attackImpulse, ForceMode2D.Impulse);
            }
            else if (playerData.attackComboStep == AttackCombo.Attack2)
            {
                rb.AddForce(Vector2.right * playerData.attackImpulse, ForceMode2D.Impulse);
            }
        }
    }

    void Update()
    {
        attackComboCounter -= Time.deltaTime;
        attackCooldownCounter -= Time.deltaTime;
        attackBufferCounter -= Time.deltaTime;

        if (attackCooldownCounter < 0.01f && attackBufferCounter > 0.01f)
        {
            rb.velocity = Vector2.zero;
            attackCooldownCounter = playerData.attackCooldownTime;
            playerData.attacking = true;

            if (attackComboCounter > 0.01f)
            {
                switch (playerData.attackComboStep)
                {
                    case AttackCombo.Attack1:
                        playerData.attackComboStep = AttackCombo.Attack2;
                        StartCoroutine(AttackImpulse());
                        break;
                    case AttackCombo.Attack2:
                        playerData.attackComboStep = AttackCombo.Attack3;
                        StartCoroutine(AttackImpulse());
                        break;
                    default:
                        playerData.attackComboStep = AttackCombo.Attack1;
                        StartCoroutine(AttackImpulse());
                        break;
                }
            }
            else
            {
                playerData.attackComboStep = AttackCombo.Attack1;
                StartCoroutine(AttackImpulse());
            }

            attackComboCounter = playerData.attackComboTime;
        }

        if (attackCooldownCounter < 0.01f)
        {
            playerData.attacking = false;
        }
    }
}
