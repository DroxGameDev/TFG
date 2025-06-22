using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttackInfo : MonoBehaviour
{
    public Arrow origin;
    public Rigidbody2D rb;
    [HideInInspector] public int damage;
    [HideInInspector] public float damageKnockback;
}
