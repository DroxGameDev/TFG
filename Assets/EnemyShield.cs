using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    EnemyData enemyData;

    [Range(0, 10f)] public float blockPushAmount;

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }


    public void OnBlock()
    {
        enemyData.player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * blockPushAmount, ForceMode2D.Impulse);
    }
}
