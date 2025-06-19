using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour, IDamageable
{
    EnemyData enemyData;
    Rigidbody2D rb;

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
    }

    public void OnDamage(int amount, float knockbackAmount, bool originFacingRight)
    {
        if (amount == 1) enemyData.damageType = DamageType.light;
        else enemyData.damageType = DamageType.hard;

        Vector2 knockbackDirection;

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

        StartCoroutine(DamageRutine(knockbackDirection, knockbackAmount));
    }
    public void OnDie()
    {
        HitStop.Instance.Stop(enemyData.hitTime);
        StartCoroutine(DestroyWait());
    }

    IEnumerator DamageRutine(Vector2 direction, float knockback)
    {
        enemyData.damaged = true;
        rb.AddForce(direction * knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(enemyData.damageWait);

        enemyData.damaged = false;
    }
    IEnumerator DestroyWait()
    {
        while (Time.timeScale != 1f)
            yield return null;
        Destroy(this.gameObject);
    }
}
