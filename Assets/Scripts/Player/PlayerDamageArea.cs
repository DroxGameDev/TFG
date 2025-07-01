using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageArea : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerDie playerDie;
    public PlayerDamage origin;

    void Start()
    {
        playerData = origin.GetComponent<PlayerData>();
        playerDie = origin.GetComponent<PlayerDie>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyAttackInfo attackInfo = collision.gameObject.GetComponent<EnemyAttackInfo>();

            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback, attackInfo.isFacingRight);
        }

        if (collision.tag == "Arrow")
        {
            ArrowAttackInfo attackInfo = collision.gameObject.GetComponent<ArrowAttackInfo>();
            bool movingRight = attackInfo.rb.velocity.x >= 0f;
            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback, movingRight);
            attackInfo.origin.EarlyDestroy();
        }

        if (collision.tag == "Spikes")
        {
            if(!playerData.dead)
            {
                playerDie.Die();
            }
        }

    }
}
