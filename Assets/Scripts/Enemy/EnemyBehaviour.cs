using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public bool gizmosActive;
    private enum State { Idle, Patrolling, Chasing, PreparingAttack,Attacking, AttackCooldown  }
    private State currentState = State.Idle;
    private Rigidbody2D rb;
    private EnemyData enemyData;
    private EnemyAttackInfo attackInfo;
    private EnemyShoot enemyShoot;
    private float idleTimer = 0f;
    private float patrolTimer = 0f;
    private float prepareAttackTimer = 0f;
    private float attackTimer;
    private float attackCooldownTimer = 0f;

    private bool playerInSight;
    private bool playerInAttackRange;
    //private bool wasHitByRanged;


    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        rb = GetComponent<Rigidbody2D>();
        enemyShoot = GetComponent<EnemyShoot>();
        setTimer();
        attackInfo = enemyData.attackOrigin.GetComponent<EnemyAttackInfo>();
        attackInfo.damage = enemyData.attackDamage;
        attackInfo.damageKnockback = enemyData.attackDamageKnockback;
        attackInfo.isFacingRight = enemyData.isFacingRight;
    }

    void Update()
    {
        if (!enemyData.visibility && enemyData.cullingEnabled) return;

        UpdatePerception();
        
        attackCooldownTimer -= Time.deltaTime;

        if (enemyData.stationaty)
        {
            StationaryBehaviour();
        }
        else
        {
            MovableBehaviour();
        }
    }

    private void StationaryBehaviour()
    {
        switch (currentState)
            {
                case State.Idle:
                    if (playerInAttackRange)
                    {
                        if (attackCooldownTimer <= 0f)
                        {
                            currentState = State.PreparingAttack;
                            setTimer();
                            enemyData.prepareAttack = true;
                            if (!CheckOrientationToPlayer()) enemyData.Flip();
                        }
                        else
                        {
                            currentState = State.AttackCooldown;
                            if (!CheckOrientationToPlayer()) enemyData.Flip();
                        }
                    }
                    break;
                case State.PreparingAttack:
                    PrepareAttack();
                    if (prepareAttackTimer <= 0f)
                    {
                        currentState = State.Attacking;
                        setTimer();
                        enemyData.prepareAttack = false;
                        enemyData.attacking = true;
                        ShootArrow();
                    }
                    break;
                case State.Attacking:
                    Attack();
                    if (attackTimer <= 0f)
                    {
                        currentState = State.AttackCooldown;
                        setTimer();
                        enemyData.attacking = false;
                    }
                    break;
                case State.AttackCooldown:
                    AttackCooldown();
                    if (attackCooldownTimer <= 0f)
                    {
                        currentState = State.PreparingAttack;
                        setTimer();
                        enemyData.prepareAttack = true;
                        if (!CheckOrientationToPlayer()) enemyData.Flip();
                    }

                    if (!playerInAttackRange)
                    {
                        currentState = State.Idle;
                    }
                    break;
            }
    }

    private void MovableBehaviour()
    {
        switch (currentState)
        {
            case State.Idle:
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    currentState = State.Patrolling;
                    setTimer();
                    enemyData.walking = true;
                }
                if (playerInSight)
                {
                    currentState = State.Chasing;
                    enemyData.walking = false;
                    enemyData.running = true;
                }
                break;

            case State.Patrolling:
                Patrol();
                if (patrolTimer <= 0)
                {
                    currentState = State.Idle;
                    setTimer();
                    enemyData.walking = false;
                }
                if (playerInSight)
                {
                    currentState = State.Chasing;
                    enemyData.walking = false;
                    enemyData.running = true;
                }
                break;

            case State.Chasing:
                Chase();
                if (playerInAttackRange)
                {
                    if (attackCooldownTimer <= 0f)
                    {
                        currentState = State.PreparingAttack;
                        setTimer();
                        enemyData.running = false;
                        enemyData.prepareAttack = true;
                        if (!CheckOrientationToPlayer()) enemyData.Flip();
                    }

                    else
                    {
                        currentState = State.AttackCooldown;
                        enemyData.running = false;
                        if (!CheckOrientationToPlayer()) enemyData.Flip();
                    }

                }
                if (!playerInSight)
                {
                    currentState = State.Patrolling;
                    setTimer();
                    enemyData.walking = true;
                    enemyData.running = false;
                    enemyData.prepareAttack = false;
                }
                break;

            case State.PreparingAttack:
                PrepareAttack();
                if (prepareAttackTimer <= 0f)
                {
                    currentState = State.Attacking;
                    setTimer();
                    enemyData.prepareAttack = false;
                    enemyData.attacking = true;
                    ShootArrow();
                }
                break;

            case State.Attacking:
                Attack();
                if (attackTimer <= 0f)
                {
                    currentState = State.AttackCooldown;
                    setTimer();
                    enemyData.attacking = false;
                }
                break;

            case State.AttackCooldown:
                AttackCooldown();
                if (attackCooldownTimer <= 0f)
                {
                    currentState = State.PreparingAttack;
                    setTimer();
                    enemyData.prepareAttack = true;
                    if (!CheckOrientationToPlayer()) enemyData.Flip();
                }

                if (!playerInAttackRange)
                {
                    currentState = State.Chasing;
                    enemyData.running = true;
                }
                break;
        }
    }
    private void UpdatePerception()
    {
        Vector2 center = transform.position;

        Vector2 sightBottonLeft = new Vector2(center.x - enemyData.detectionRangeX, center.y - enemyData.detectionRangeY);
        Vector2 sightTopRight = new Vector2(center.x + enemyData.detectionRangeX, center.y + enemyData.detectionRangeY);

        playerInSight = (bool)Physics2D.OverlapArea(sightBottonLeft, sightTopRight, enemyData.playerLayer);

        Vector2 attackBottomLeft = new Vector2(center.x - enemyData.attackRangeX, center.y - enemyData.attackRangeY);
        Vector2 attackTopRight = new Vector2(center.x + enemyData.attackRangeX, center.y + enemyData.attackRangeY);

        playerInAttackRange = (bool)Physics2D.OverlapArea(attackBottomLeft, attackTopRight, enemyData.playerLayer);

        //check if obstacle between enemy and player

        if (playerInSight)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, enemyData.player.transform.position);

            Vector2 direction = (enemyData.player.transform.position - transform.position).normalized;

            if (Physics2D.Raycast(transform.position, direction, distanceToPlayer, enemyData.obstacleLayer))
            {
                playerInSight = false;
                playerInAttackRange = false;
            }
        }
    }

    private void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        EnemyMove(enemyData.walkSpeed);

        // Detectar borde
        bool groundAhead = Physics2D.Raycast(enemyData.groundCheck.position, Vector2.down, 0.5f, enemyData.groundLayer);
        bool wallAhead = Physics2D.Raycast(enemyData.wallCheck.position, Vector2.right * enemyData.moveDirection, 0.1f, enemyData.groundLayer);

        if ((!groundAhead || wallAhead) && !(rb.velocity.y < 0f))
            enemyData.Flip();
    }

    private void Chase()
    {
        float dir = Mathf.Sign(enemyData.player.position.x - transform.position.x);

        if (dir != enemyData.moveDirection) enemyData.Flip();

        EnemyMove(enemyData.runSpeed);
    }
    private void PrepareAttack()
    {
        prepareAttackTimer -= Time.deltaTime;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }
    private void Attack()
    {
        attackTimer -= Time.deltaTime;
        rb.velocity = new Vector2(0f, rb.velocity.y);

    }
    private void AttackCooldown()
    {
        //attackCooldownTimer -= Time.deltaTime;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void ShootArrow()
    {
        if (enemyShoot != null)
        {
            enemyShoot.Shoot();
        }
    }

    private void setTimer()
    {
        switch (currentState)
        {
            case State.Patrolling:
                patrolTimer = Random.Range(enemyData.minPatrolTime, enemyData.maxPatrolTime);
                break;
            case State.Idle:
                idleTimer = Random.Range(enemyData.minIdleTime, enemyData.maxIdleTime);
                break;
            case State.PreparingAttack:
                prepareAttackTimer = enemyData.prepareAttackTime;
                break;
            case State.Attacking:
                attackTimer = enemyData.attackTime;
                break;
            case State.AttackCooldown:
                attackCooldownTimer = enemyData.attackCooldownTime;
                break;

        }

    }

    private bool CheckOrientationToPlayer()
    {
        return (enemyData.isFacingRight && (transform.position.x < enemyData.player.position.x)) ||
                (!enemyData.isFacingRight && (transform.position.x > enemyData.player.position.x));
    }

    private void EnemyMove(float speed)
    {
        if (!enemyData.damaged)
            rb.velocity = new Vector2(enemyData.moveDirection * speed, rb.velocity.y);
    }
    
    private void OnDrawGizmos()
    {
        if (gizmosActive)
        {

            Gizmos.color = Color.blue;

            Vector2 center = transform.position;
            float width = enemyData.detectionRangeX;
            float height = enemyData.detectionRangeY;

            Vector2 bottomLeft = new Vector2(center.x - width, center.y - height);
            Vector2 topRight = new Vector2(center.x + width, center.y + height);

            Gizmos.DrawLine(bottomLeft, topRight);

            Gizmos.color = Color.red;
            width = enemyData.attackRangeX;
            height = enemyData.attackRangeY;

            Vector2 attackBottomLeft = new Vector2(center.x - width, center.y - height);
            Vector2 attackTopRight = new Vector2(center.x + width, center.y + height);

            Gizmos.DrawLine(attackBottomLeft, attackTopRight);
            /*
            Vector2 positon = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = rb.velocity+positon;
            Gizmos.DrawLine(transform.position, direction);
            */

        }
    }
}
