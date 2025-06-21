using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Arrow : AffectedByGravity
{
    private SpriteRenderer sprite;
    private Animator anim;
    public Collider2D attackCollider;
    public Collider2D obstaclesCollider;
    public ArrowAttackInfo attackInfo;
    public GameObject speed;
    private float arrowSpeed;
    private float shootTimer;
    private float receivedShooTime;
    public bool returning;
    protected override void OnEnable()
    {
        base.OnEnable();
        constForce.force = new Vector2(0f, 0f);
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void ShootArrow(Vector2 direction, float speed, float shootTime, int damage, float knockback)
    {
        arrowSpeed = speed;
        receivedShooTime = shootTime;
        shootTimer = shootTime;
        attackInfo.damage = damage;
        attackInfo.damageKnockback = knockback;

        SetArrow(direction, arrowSpeed);
        StartCoroutine(StartShootCountdown());
    }
    public void ReturnArrow(Vector2 direction, float speedMult)
    {
        shootTimer = receivedShooTime;
        returning = true;
        speed.transform.localScale = new Vector3(speed.transform.localScale.x * speedMult,speed.transform.localScale.y,speed.transform.localScale.z);
        SetArrow(direction, arrowSpeed * speedMult);
    }

    private void SetArrow(Vector2 direction, float speed)
    {
        //velocity
        rb.velocity = new Vector2(speed * direction.x, speed * direction.y);
        //sprite direction
        // Compute angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate arrow assuming sprite looks to the right; adjust offset if not
        float spriteAngleOffset = 0f;
        transform.rotation = Quaternion.Euler(0, 0, angle + spriteAngleOffset);
    }

    IEnumerator StartShootCountdown()
    {
        while (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        anim.SetBool("Despawn", true);
    }

    public void Collision()
    {
        anim.SetBool("Despawn", true);
    }

    public void EarlyDestroy()
    {
        ResetArrow();
    }

    private void ResetArrow()
    {
        Destroy(this.gameObject);
    }

    void Update()
    {

    }
}
