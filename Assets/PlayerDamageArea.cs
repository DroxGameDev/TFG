using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageArea : MonoBehaviour
{
    public PlayerDamage origin;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyAttackInfo attackInfo = collision.gameObject.GetComponent<EnemyAttackInfo>();
            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback, attackInfo.isFacingRight);
        }

    }
}
