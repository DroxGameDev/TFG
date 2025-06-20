using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedByGravity : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector]public ConstantForce2D constForce;
    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        constForce = GetComponent<ConstantForce2D>();
        constForce.force = new Vector2(0f, Physics2D.gravity.y * rb.mass);
    }
}
