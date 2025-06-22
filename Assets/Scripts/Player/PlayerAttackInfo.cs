using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackInfo : MonoBehaviour
{
    public PlayerAttack playerAttack;
    [HideInInspector] public int damage;
    [HideInInspector] public float damageKnockback;
    [HideInInspector] public bool burningPewter = false;
    [HideInInspector] public bool isFacingRight;

    public void DoneDamage()
    {
        playerAttack.AttackDoneFeedback();
    }
}
