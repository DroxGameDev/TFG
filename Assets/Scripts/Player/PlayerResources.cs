using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private PlayerData playerData;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Resources")]
    public int coins;

    [Header("Coin")]
    public GameObject CoinPrefab;
    public List<GameObject> nearbyCoins;
    private float showCoinCounter;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        nearbyCoins = new List<GameObject>();
    }

    public void GetCoin(Collider2D collision)
    {
        if (collision.tag == "Coin" && !playerData.showingCoin)
        {
            nearbyCoins.Add(collision.gameObject);
        }
    }

    public void RemoveCoin(Collider2D collision)
    {
        if (collision.tag == "Coin" && !playerData.showingCoin)
        {
            nearbyCoins.Remove(collision.gameObject);
        }
    }


    public void PickDropCoin()
    {
        //pick coin
        if (nearbyCoins.Count > 0 && !playerData.movingWithPowers && !playerData.showingCoin)
        {
            coins++;
            GameObject coinToRemove = nearbyCoins[0];
            nearbyCoins.Remove(coinToRemove);
            Destroy(coinToRemove);
        }
        //drop coin
        else if (nearbyCoins.Count == 0 && coins > 0 && !playerData.showingCoin)
        {
            coins--;
            GameObject newCoin = CoinPrefab;
            newCoin.transform.position = transform.position;
            Instantiate(newCoin);
        }

        else if (playerData.showingCoin && nearbyCoins.Count == 0)
        {
            nearbyCoins.Add(playerData.showedCoin);
            CoinGone(playerData.showedCoin.GetComponent<Coin>());
        }

    }

    public void ShowCoin()
    {
        if (coins > 0 && !playerData.showingCoin && !playerData.burningIron)
        {
            //rb.velocity = Vector2.zero;
            GameObject newCoin = Instantiate(CoinPrefab, playerData.shootPoint.position, Quaternion.identity);
            playerData.showedCoin = newCoin;
            Coin coinScript = newCoin.GetComponent<Coin>();
            CoinInHand(coinScript);

            coins--;
            showCoinCounter = playerData.showCoinTime;
            playerData.showingCoin = true;
        }
    }

    public void CoinInHand(Coin coin)
    {
        coin.setGravity(0f, 0f);
    }

    public void CoinGone(Coin coin)
    {
        coin.setGravity(0f, 1f);
        playerData.showedCoin = null;
        playerData.showingCoin = false;
    }

    void Update()
    {
        if (playerData.showingCoin)
        {
            playerData.showedCoin.transform.position = playerData.shootPoint.position;
            showCoinCounter -= Time.deltaTime;

            if (showCoinCounter < 0.01f)
            {
                coins++;
                GameObject coinToRemove =  playerData.showedCoin;
                playerData.showedCoin = null;
                Destroy(coinToRemove);
                playerData.showingCoin = false;
            }
        }
    }
}
