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

        

        for (int i = 0; i < enemyData.drops.Length; i++)
        {
            GameObject drop = PickupsSpawns.Instance.SpawnPickUp(enemyData.drops[i], enemyData.transform.position);

            float xDirection = Random.Range(-0.5f, 0.5f);
            Vector2 expulseDropDirection = new Vector2(xDirection, 1);
            drop.GetComponent<Rigidbody2D>().AddForce(expulseDropDirection * enemyData.dropDispersion, ForceMode2D.Impulse);
        }
        

        Destroy(this.gameObject);
    }
}
