using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public BreakeableObject origin;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            AttackInfo attackInfo = collision.gameObject.GetComponent<AttackInfo>();
            if (tag == "BreakableObject")
            {
                if (attackInfo.burningPewter)
                    origin.OnDamage(attackInfo.damage);
                else
                    origin.OnDamage(0);
            }
            
        }
    }
}
