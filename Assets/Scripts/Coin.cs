using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AffectedByGravity
{
    [HideInInspector] public Collider2D triggerCollider;

    public Vector2 velocity;
    void Start()
    {
        OnStart();
        triggerCollider = GetComponent<Collider2D>();
        velocity = rb.velocity;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
