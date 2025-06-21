using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    EnemyData enemyData;

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }
    
    public float blockAngle = 120f; // En grados

    public bool IsBlocking()
    {
        Vector3 directionToPlayer = (enemyData.player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        return angle < blockAngle / 2f;
    }
}
