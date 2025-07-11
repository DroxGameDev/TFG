using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    EnemyData enemyData;
    EnemyDie enemyDie;
    EnemyShield enemyShield;
    Rigidbody2D rb;

    CinemachineImpulseSource impulseSource;

    [HideInInspector]
    public int Health
    {
        get => enemyData.health;
        set => enemyData.health = value;
    }

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        rb = GetComponent<Rigidbody2D>();
        enemyShield = GetComponent<EnemyShield>();
        enemyDie = GetComponent<EnemyDie>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void OnDamage(int amount, float knockbackAmount, bool originFacingRight)
    {
        if (amount == 1) enemyData.damageType = DamageType.light;
        else enemyData.damageType = DamageType.hard;

        Vector2 knockbackDirection;

        if (enemyShield != null)
        {
            if (((originFacingRight && !enemyData.isFacingRight) || (!originFacingRight && enemyData.isFacingRight)) &&
                !enemyData.prepareAttack && !enemyData.attacking)
            {
                enemyShield.OnBlock();
                return;
            }
        }

        if (originFacingRight)
        {
            if (enemyData.isFacingRight && !enemyData.prepareAttack && !enemyData.attacking)
            {
                enemyData.Flip();
            }
            knockbackDirection = Vector2.right;
        }
        else
        {
            if (!enemyData.isFacingRight && !enemyData.prepareAttack && !enemyData.attacking)
            {
                enemyData.Flip();
            }
            knockbackDirection = Vector2.left;
        }

        enemyData.health -= amount;
        if (enemyData.health <= 0)
            OnDie();


        if (amount != 2)
        {
            GameManager.Instance.player.GetComponent<PlayerData>().PlayerAttackSFX();
        }
        CameraManager.Instance.CameraShake(impulseSource);
        StartCoroutine(DamageRutine(knockbackDirection, knockbackAmount));
    }
    public void OnDie()
    {
        HitStop.Instance.Stop(enemyData.hitTime);
        enemyDie.Die();
    }

    IEnumerator DamageRutine(Vector2 direction, float knockback)
    {
        enemyData.damaged = true;

        enemyData.sprite.material.SetFloat("_FlashAmount", 1);
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(enemyData.damageWait);

        enemyData.sprite.material.SetFloat("_FlashAmount", 0);
        enemyData.damaged = false;
    }
    
}
