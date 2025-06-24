using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class EnemyDamageArea : MonoBehaviour
{
    public EnemyDamage origin;
    public EnemyDie enemyDie;

    void Start()
    {
        enemyDie = origin.GetComponent<EnemyDie>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerAttackInfo attackInfo = collision.gameObject.GetComponent<PlayerAttackInfo>();
            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback, attackInfo.isFacingRight);
            attackInfo.DoneDamage();
        }

        if (collision.tag == "Coin")
        {
            CoinAttackInfo coin = collision.gameObject.GetComponent<CoinAttackInfo>();

            if (coin != null)
            {
                bool movingRight = coin.rb.velocity.x >= 0f;
                Destroy(coin.origin.gameObject);
                origin.OnDamage(coin.damage, coin.damageknockback, movingRight);
            }
        }

        if (collision.tag == "Arrow")
        {
            ArrowAttackInfo arrow = collision.gameObject.GetComponent<ArrowAttackInfo>();

            if (arrow != null && arrow.origin.returning)
            {
                bool movingRight = arrow.rb.velocity.x >= 0f;
                origin.OnDamage(arrow.damage, arrow.damageKnockback, movingRight);
            }
        }

        if (collision.tag == "Spikes")
        {
            enemyDie.Die();
        }
    }
}
