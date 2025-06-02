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

    [Header("Metal Reserves")]
    [Range(0f, 60f)] public float ironReserve;
    [Range(0f, 60f)] public float steelReserve;
    [Range(0f, 60f)] public float tinReserve;
    [Range(0f, 60f)] public float pewterReserve;
    [Header("Empty Reserves Checkers")]
    public bool ironEmpty;
    public bool steelEmpty;
    public bool tinEmpty;
    public bool pewterEmpty;


    [Header("Coin")]
    public GameObject CoinPrefab;
    private float showCoinCounter;

    [Space(10)]
    public List<GameObject> nearbyItems;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        nearbyItems = new List<GameObject>();
        StartMetalReserves();
        checkIfEmpty();
    }

    public void GetObject(Collider2D collision)
    {
        if ((collision.tag == "Coin" || collision.tag == "Vial") && !playerData.showingCoin)
        {
            nearbyItems.Add(collision.gameObject);
        }
    }

    public void RemoveObject(Collider2D collision)
    {
        if ((collision.tag == "Coin" || collision.tag == "Vial") && !playerData.showingCoin)
        {
            nearbyItems.Remove(collision.gameObject);
        }
    }


    public void PickDropCoinInput()
    {
        //pick object
        if (nearbyItems.Count > 0 && !playerData.movingWithPowers && !playerData.showingCoin)
        {
            if (nearbyItems[0].tag == "Coin")
            {
                UpdateCoins(1);
            }
            else if (nearbyItems[0].tag == "Vial")
            {
                Vial vial = nearbyItems[0].GetComponent<Vial>();

                PickVialUpdateMetalReserves(vial);
            }
            GameObject objectToRemove = nearbyItems[0];
            nearbyItems.Remove(objectToRemove);
            Destroy(objectToRemove);
        }
        //drop coin
        else if (nearbyItems.Count == 0 && coins > 0 && !playerData.showingCoin)
        {
            UpdateCoins(-1);
            GameObject newCoin = CoinPrefab;
            newCoin.transform.position = transform.position;
            Instantiate(newCoin);
        }

        else if (playerData.showingCoin && nearbyItems.Count == 0)
        {
            nearbyItems.Add(playerData.showedCoin);
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
        
        checkIfEmpty();
        BurningUpdateMetalReserves();
    }
    #region reserves update
    void StartMetalReserves()
    {
        if (ironReserve < 0.01f && ironVials > 0)
        {
            ironReserve += 60f;
            ironVials--;
        }

        if (steelReserve < 0.01f && steelVials > 0)
        {
            steelReserve += 60f;
            steelVials--;
        }

        if (tinReserve < 0.01f && tinVials > 0)
        {
            tinReserve += 60f;
            tinVials--;
        }

        if (pewterReserve < 0.01f && pewterVials > 0)
        {
            pewterReserve += 60f;
            pewterVials--;
        }
    }
    void checkIfEmpty()
    {
        if (ironReserve <= 0.01f && !ironEmpty)
        {
            ironEmpty = true;
        }

        if (steelReserve <= 0.01f && !steelEmpty)
        {
            steelEmpty = true;
        }

        if (tinReserve <= 0.01f && !tinEmpty)
        {
            tinEmpty = true;
        }   
        
        if (pewterReserve <= 0.01f && !pewterEmpty)
        {
            pewterEmpty = true;
        }
    }

    void BurningUpdateMetalReserves()
    {
        if (!ironEmpty && playerData.burningIron)
        {
            ironReserve -= Time.deltaTime;

            if (ironReserve <= 0.01 && ironVials > 0)
            {
                ironReserve += 60f;
                ironVials--;
            }
        }

        if (!steelEmpty && playerData.burningSteel)
        {
            steelReserve -= Time.deltaTime;

            if (steelReserve <= 0.01 && steelVials > 0)
            {
                steelReserve += 60f;
                steelVials--;
            }
        }

        if (!tinEmpty && playerData.burningTin)
        {
            tinReserve -= Time.deltaTime;

            if (tinReserve <= 0.01 && tinVials > 0)
            {
                tinReserve += 60f;
                tinVials--;
            }
        }
        
        if (!pewterEmpty && playerData.burningPewter)
        {
            pewterReserve -= Time.deltaTime;

            if (pewterReserve <= 0.01 && pewterVials > 0)
            {
                pewterReserve += 60f;
                pewterVials--;
            }
        }
    }
    void PickVialUpdateMetalReserves(Vial vial)
    {
        switch (vial.type)
        {
            case VialType.iron:
                if (ironEmpty)
                {
                    ironReserve += 60f;
                    ironEmpty = false;
                }
                else ironVials++;
                break;
            case VialType.steel:
                if (steelEmpty)
                {
                    steelReserve += 60f;
                    steelEmpty = false;
                }
                else steelVials++;
                break;
            case VialType.tin:
                if (tinEmpty)
                {
                    tinReserve += 60f;
                    tinEmpty = false;
                }
                else tinVials++;
                break;
            case VialType.pewter:
                if (pewterEmpty)
                {
                    pewterReserve += 60f;
                    pewterEmpty = false;
                }
                else pewterVials++;
                break;
        }
    }
    #endregion

    public void IronItem(GameObject item)
    {
        if (item.tag == "Coin")
        {
            UpdateCoins(1);
        }
        else if (item.tag == "Vial")
        {
            Vial vial = item.GetComponent<Vial>();
            PickVialUpdateMetalReserves(vial);
        }
        Destroy(item);
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
