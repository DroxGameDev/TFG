using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AffectedByGravity
{
    public Collider2D attackCollider;
    public Transform checkGroundPosition;

    public void setGravity(float newGracityScaleX, float newGracityScaleY)
    {
        ConstantForce2D forceMode = GetComponent<ConstantForce2D>();
        forceMode.force = new Vector2(Physics2D.gravity.x * newGracityScaleX * rb.mass, Physics2D.gravity.y * newGracityScaleY * rb.mass);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        attackCollider.enabled = false;
    }
}
