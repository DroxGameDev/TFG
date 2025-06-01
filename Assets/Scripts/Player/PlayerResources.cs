using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private PlayerData playerData;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Resources")]
    public int coins;
    public int ironVials;
    public int steelVials;
    public int tinVials;
    public int pewterVials;

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

    public void GetObject(Collider2D collision)
    {
        if (collision.tag == "Coin" && !playerData.showingCoin)
        {
            nearbyCoins.Add(collision.gameObject);
        }
    }

    public void RemoveObject(Collider2D collision)
    {
        if (collision.tag == "Coin" && !playerData.showingCoin)
        {
            nearbyCoins.Remove(collision.gameObject);
        }
    }


    public void PickDropCoinInput()
    {
        //pick coin
        if (nearbyCoins.Count > 0 && !playerData.movingWithPowers && !playerData.showingCoin)
        {
            UpdateCoins(1);
            GameObject coinToRemove = nearbyCoins[0];
            nearbyCoins.Remove(coinToRemove);
            Destroy(coinToRemove);
        }
        //drop coin
        else if (nearbyCoins.Count == 0 && coins > 0 && !playerData.showingCoin)
        {
            UpdateCoins(-1);
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

    public void ShowCoinInput()
    {
        if (coins > 0 && !playerData.showingCoin && !playerData.burningIron)
        {
            //rb.velocity = Vector2.zero;
            GameObject newCoin = Instantiate(CoinPrefab, playerData.shootPoint.position, Quaternion.identity);
            playerData.showedCoin = newCoin;
            Coin coinScript = newCoin.GetComponent<Coin>();
            CoinInHand(coinScript);

            UpdateCoins(-1);
            showCoinCounter = playerData.showCoinTime;
            playerData.showingCoin = true;
        }
    }

    void CoinInHand(Coin coin)
    {
        coin.setGravity(0f, 0f);
    }

    void CoinGone(Coin coin)
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
                UpdateCoins(1);
                GameObject coinToRemove = playerData.showedCoin;
                playerData.showedCoin = null;
                Destroy(coinToRemove);
                playerData.showingCoin = false;
            }
        }
    }

    public void IronCoin(GameObject coin)
    {
        UpdateCoins(1);
        Destroy(coin);
    }

    public void SteelCoin(Coin coin)
    {
        CoinGone(coin);
    }

    void UpdateCoins(int amount)
    {
        coins += amount;
        CoinCounter.instance.UpdateCoins(coins);
    }
}
