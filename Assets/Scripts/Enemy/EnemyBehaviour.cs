using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public bool gizmosActive;
    private enum State { Idle, Patrolling, Chasing, Attacking, Cooldown, Stunned, Dead }
    private State currentState = State.Idle;
    private Rigidbody2D rb;
    private EnemyData enemyData;
    //private float cooldownTimer = 0f;
    private float idleTimer = 0f;
    private float patrolTimer = 0f;

    private bool playerInSight;
    private bool playerInAttackRange;
    private bool wasHitByRanged;


    void Start()
    {
        enemyData = GetComponent<EnemyData>();
        rb = GetComponent<Rigidbody2D>();
        setTimer();
    }

    void Update()
    {
        UpdatePerception();

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
                if (playerInSight || wasHitByRanged)
                {
                    enemyData.walking = false;
                    enemyData.running = true;
                    currentState = State.Chasing;
                }
                break;

            case State.Patrolling:
                Patrol();
                patrolTimer -= Time.deltaTime;
                if (patrolTimer <= 0)
                {
                    currentState = State.Idle;
                    setTimer();
                    enemyData.walking = false;
                }
                if (playerInSight || wasHitByRanged)
                {
                    enemyData.walking = false;
                    enemyData.running = true;
                    currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                Chase();
                //if (playerInAttackRange) currentState = State.Attacking;
                if (!playerInSight && !wasHitByRanged)
                {
                    currentState = State.Patrolling;
                    setTimer();
                    enemyData.walking = true;
                    enemyData.running = false;
                }
                break;
        }
    }

    private void UpdatePerception()
    {
        Vector2 center = transform.position;
        float width = enemyData.detectionRangeX;
        float height = enemyData.detectionRangeY;

        Vector2 bottomLeft = new Vector2(center.x - width, center.y - height);
        Vector2 topRight = new Vector2(center.x + width, center.y + height);

        playerInSight = (bool)Physics2D.OverlapArea(bottomLeft, topRight, enemyData.playerLayer);

        //check if obstacle between enemy and player

        if (playerInSight)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, enemyData.player.transform.position);

            Vector2 direction = (enemyData.player.transform.position - transform.position).normalized;

            if (Physics2D.Raycast(transform.position, direction, distanceToPlayer, enemyData.groundLayer))
                playerInSight = false;
        }
    }

    private void Patrol()
    {
        EnemyMove(enemyData.walkSpeed);

        // Detectar borde
        bool groundAhead = Physics2D.Raycast(enemyData.groundCheck.position, Vector2.down, 0.1f, enemyData.groundLayer);
        bool wallAhead = Physics2D.Raycast(enemyData.wallCheck.position, Vector2.right * enemyData.moveDirection, 0.1f, enemyData.groundLayer);

        if (!groundAhead || wallAhead)
            enemyData.Flip();
    }

    private void Chase()
    {
        float dir = Mathf.Sign(enemyData.player.position.x - transform.position.x);
        enemyData.moveDirection = dir;
        EnemyMove(enemyData.runSpeed);
        transform.localScale = new Vector3(enemyData.moveDirection, 1, 1);
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
                
        }

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
            /*
            Vector2 positon = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = rb.velocity+positon;
            Gizmos.DrawLine(transform.position, direction);
            */

        }
    }
}
