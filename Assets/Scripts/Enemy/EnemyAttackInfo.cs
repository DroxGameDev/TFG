using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackInfo : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public float damageKnockback;
    [HideInInspector] public bool isFacingRight = true;
}
