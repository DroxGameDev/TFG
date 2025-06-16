using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyData enemyData;

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }
}
