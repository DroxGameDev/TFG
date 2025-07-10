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
    private PlayerAttackInfo attackInfo;
    private float prepareAttackCounter;
    private float attackCounter;
    private float attackBufferCounter;
    private float attackCooldownTime;
    private float attackCooldownCounter;
    private float attackComboCounter;
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        attackInfo = playerData.attackOrigin.GetComponent<PlayerAttackInfo>();
        attackCooldownTime = playerData.prepareAttackTime + playerData.attackTime + playerData.attackCooldownOvertime;
    }
    public void OnAttack()
    {
        attackBufferCounter = playerData.attackBufferTime;
    }

    private IEnumerator Attack()
    {
        attackInfo.damage = playerData.damage;
        attackInfo.damageKnockback = playerData.damageKnockback;

        prepareAttackCounter = playerData.prepareAttackTime;
        playerData.preparingAttack = true;

        while (prepareAttackCounter > 0f)
        {
            prepareAttackCounter -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        playerData.preparingAttack = false;
        playerData.attacking = true;

        attackCounter = playerData.attackTime;

        while (attackCounter > 0f)
        {
            attackCounter -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        playerData.attacking = false;
        attackComboCounter = playerData.attackComboTime;
    }

    void Update()
    {
        attackCooldownCounter -= Time.deltaTime;
        attackBufferCounter -= Time.deltaTime;

        if (attackCooldownCounter < 0.01f && attackBufferCounter > 0.01f)
        {
            if (rb.velocity.y > 0) //force to the grown if the palyer is mid air
            {
                rb.AddForce(Vector2.down * playerData.velocity.y * (1 - playerData.jumpCutMultiplier), ForceMode2D.Impulse);
            }

            attackCooldownCounter = attackCooldownTime;
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
                    case AttackCombo.Attack3:
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
        }
    }

    public void AttackDoneFeedback()
    {
        if (playerData.burningPewter)
        {
            SoundEffectManager.instance.PlayRandomSoundFXClip(playerData.punchAttackClips, playerData.attackOrigin.transform, playerData.attackVFXvolume);
        }
        else
        {
            SoundEffectManager.instance.PlayRandomSoundFXClip(playerData.knifeAttackClips, playerData.attackOrigin.transform, playerData.attackVFXvolume);
        }

        if (playerData.isFacingRight)
        {
            rb.AddForce(Vector2.left * playerData.attackImpulse, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.right * playerData.attackImpulse, ForceMode2D.Impulse);
        }
    }
}
