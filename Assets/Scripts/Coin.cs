using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AffectedByGravity
{
    [HideInInspector] public Collider2D triggerCollider;

    public Vector2 velocity;
    void OnEnable()
    {
        OnStart();
        triggerCollider = GetComponent<Collider2D>();
        velocity = rb.velocity;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

    }
    
    public void setGravity(float newGracityScaleX, float newGracityScaleY)
    {
        ConstantForce2D forceMode = GetComponent<ConstantForce2D>();
        forceMode.force = new Vector2(Physics2D.gravity.x * newGracityScaleX * rb.mass, Physics2D.gravity.y * newGracityScaleY * rb.mass);
    }
}
