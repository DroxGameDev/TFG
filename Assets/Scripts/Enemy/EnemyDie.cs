using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    EnemyData enemyData;

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }

    public void Die()
    {
        StartCoroutine(DestroyWait());
    }
    private IEnumerator DestroyWait()
    {
        while (Time.timeScale != 1f)
            yield return null;

        int amountDrops = Random.Range(0, enemyData.maxAmountDrops);

        if (amountDrops > 0)
        {
            for (int i = 0; i < amountDrops; i++)
            {
                int newDropIndex = Random.Range(0, enemyData.posibleDrops.Length);

                GameObject drop = enemyData.posibleDrops[newDropIndex];

                if (drop.tag == "Vial")
                {
                    drop.GetComponent<Vial>().type = (VialType)Random.Range(0, 3);
                }

                PickupsSpawns.Instance.SpawnPickUp(drop, enemyData.groundCheck.position);

                float xDirection = Random.Range(-0.5f, 0.5f);
                Vector2 expulseDropDirection = new Vector2(xDirection, 1);
                drop.GetComponent<Rigidbody2D>().AddForce(expulseDropDirection * enemyData.dropDispersion, ForceMode2D.Impulse);
            }
        }

        Destroy(this.gameObject);
    }
}
