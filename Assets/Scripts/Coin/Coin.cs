using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : AffectedByGravity
{
    public Collider2D attackCollider;
    public Transform checkGroundPosition;
    public float maxSpeed;
    public GameObject speedEffect;

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

    void Update()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            float scaleValue = Mathf.InverseLerp(0f, maxSpeed, rb.velocity.magnitude);
            speedEffect.transform.localScale = new Vector3(scaleValue, 1, 1);
        }
        else
            speedEffect.transform.localScale = Vector3.one;

        Vector2 direction = rb.velocity.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate arrow assuming sprite looks to the right; adjust offset if not
        float spriteAngleOffset = 0f;
        speedEffect.transform.rotation = Quaternion.Euler(0, 0, angle + spriteAngleOffset);
    }  
}
