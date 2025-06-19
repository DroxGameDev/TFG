using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageArea : MonoBehaviour
{
    private PlayerData playerData;
    public PlayerDamage origin;

    void Start()
    {
        playerData = origin.GetComponent<PlayerData>();
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
            attackInfo.origin.EarlyDestroy();
            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback, playerData.isFacingRight);
        }

    }
}
