using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public Transform attackOrigin;
    private EnemyData enemyData;

    public GameObject arrowPrefab;
    
    [Range(0f,50f)]public float shootSpeed;
    [Range(0f,10f)]public float shootTime;
    [Range(0f,10f)]public float shootKnockback;
    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }

    public void Shoot()
    {
        Vector2 direction = enemyData.isFacingRight ? Vector2.right : Vector2.left;

        GameObject newArrow = Instantiate(arrowPrefab, attackOrigin.position, Quaternion.identity);

        newArrow.GetComponent<Arrow>().ShootArrow(direction, shootSpeed, shootTime, enemyData.attackDamage,shootKnockback);
    }
}
