using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAttackInfo : MonoBehaviour
{
    public Rigidbody2D rb;
    public int damage;
    [Range(0, 10f)] public float damageknockback;

}
