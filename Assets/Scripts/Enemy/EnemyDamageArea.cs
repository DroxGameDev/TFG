using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageArea : MonoBehaviour
{
    public EnemyDamage origin;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerAttackInfo attackInfo = collision.gameObject.GetComponent<PlayerAttackInfo>();

            origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback,attackInfo.isFacingRight);
        }
    }
}
