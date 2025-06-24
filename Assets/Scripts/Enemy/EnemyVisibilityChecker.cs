using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisibilityChecker : MonoBehaviour
{
    public EnemyData enemyData;

    void OnBecameInvisible()
    {
        enemyData.visibility = false;
        enemyData.GravitySleep();
        enemyData.rb.Sleep();
    }

    void OnBecameVisible()
    {
        enemyData.visibility = true;
        enemyData.GravityAwake();
        enemyData.rb.WakeUp();
    }
}
