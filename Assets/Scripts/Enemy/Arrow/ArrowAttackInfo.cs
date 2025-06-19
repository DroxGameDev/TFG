using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackInfo : MonoBehaviour
{
    public Arrow origin;
    [HideInInspector] public int damage;
    [HideInInspector] public float damageKnockback;
}
