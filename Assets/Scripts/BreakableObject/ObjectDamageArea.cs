using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDamageArea : MonoBehaviour
{
    public BreakeableObject origin;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerAttackInfo attackInfo = collision.gameObject.GetComponent<PlayerAttackInfo>();
            if (tag == "BreakableObject")
            {
                if (attackInfo.burningPewter)
                    origin.OnDamage(attackInfo.damage, attackInfo.damageKnockback,attackInfo.transform);
                else
                    origin.OnDamage(0, attackInfo.damageKnockback,attackInfo.transform);
            }
        }
    }
}
