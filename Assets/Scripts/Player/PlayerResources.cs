using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private PlayerData playerData;

    [Header("Resources")]
    public int coins;

    [Header("Coin")]
    public GameObject CoinPrefab;
    public List<GameObject> nearbyCoins;
    public Collider2D pickCollider;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        nearbyCoins = new List<GameObject>(); 
    }

    public void GetCoin(Collider2D collision)
    {
         if (collision.tag == "Coin")
        {
            nearbyCoins.Add(collision.gameObject);
        }
    }

    public void RemoveCoin(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            nearbyCoins.Remove(collision.gameObject);
        }
    }


    public void PickDropCoin()
    {
        
        //pick coin
        if (nearbyCoins.Count > 0 && !playerData.movingWithPowers)
        {
            coins++;
            GameObject coinToRemove = nearbyCoins[0];
            nearbyCoins.Remove(coinToRemove);
            Destroy(coinToRemove);
        }
        //drop coin
        else if (nearbyCoins.Count == 0 && coins > 0)
        {
            coins--;
            GameObject newCoin = CoinPrefab;
            
            newCoin.transform.position = transform.position;

            Instantiate(newCoin);
        }
        
    }
}
