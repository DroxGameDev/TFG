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
        if (collision.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<PlayerResources>();
            if (player != null)
            {
                player.coins += 1;
            }
        }
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
