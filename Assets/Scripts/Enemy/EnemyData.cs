using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    light,
    medium,
    hard
}

public enum EnemyType
{
    Soldier,
    Crossbow,
    Hazekiller
}

public class EnemyData : AffectedByGravity
{
    public int health;
    public EnemyType enemyType;
    public Transform player;

    [Header("Movement")]
    [Range(0f, 20f)] public float walkSpeed;
    [Range(0f, 20f)] public float runSpeed;
    [HideInInspector] public float moveDirection = 1f; // -1: left, 1: right
    [Space(10)]
    [Range(0.0f, 9.5f)] public float minIdleTime;
    [Range(0.5f, 10f)] public float maxIdleTime;
    [Range(0.0f, 9.5f)] public float minPatrolTime;
    [Range(0.5f, 10f)] public float maxPatrolTime;
    [Space(10)]

    [Header("Attack")]
    public GameObject attackOrigin;

    [Space(10)]

    [Range(0, 1f)] public float prepareAttackTime;
    [Range(0, 1f)] public float attackTime;
    [Range(0, 5f)] public float attackCooldownTime;

    [Space(10)]

    [Range(1, 5)] public int attackDamage;
    [Range(0, 10f)] public float attackDamageKnockback;

    [Space(10)]

    [Header("Check")]
    
    [Range(0f, 50f)] public float detectionRangeX;
    [Range(0f, 50f)] public float detectionRangeY;
    
    [Range(0f, 25f)] public float attackRangeX;
    [Range(0f, 25f)] public float attackRangeY;

    [Space(10)]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    [Header("Damage")]
    public DamageType damageType;
    public float damageWait;
    [Range(0, 1f)] public float hitTime;

    [Header("Animation")]
    public Animator anim;
    public bool isFacingRight = true;
    public bool walking;
    public bool running;
    public bool damaged;
    public bool prepareAttack;
    public bool attacking;

    public void Flip()
    {
        if (Time.timeScale == 1f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            moveDirection *= -1;

            attackOrigin.GetComponent<EnemyAttackInfo>().isFacingRight = !attackOrigin.GetComponent<EnemyAttackInfo>().isFacingRight;
            /*
            Vector3 currentShootPoint = playerData.shootPoint.localPosition;
            currentShootPoint.x *= -1f;
            playerData.shootPoint.localPosition = currentShootPoint;
            */
        }
    }


}

