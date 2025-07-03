using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    private PlayerData playerData;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Resources")]
    [Range(0, 10)] public int health;
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
    [Range(0f, 10f)] public float coinImpulseOnDrop;

    [Space(10)]
    public List<GameObject> nearbyItems;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        rb = GetComponent<Rigidbody2D>();
        nearbyItems = new List<GameObject>();
        UpdateCanvas();
        UpdateMetalReserves();
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

    #region  input
    public void PickDropCoinInput()
    {
        //pick object
        if (nearbyItems.Count > 0 /* && !playerData.movingWithPowers */ && !playerData.showingCoin)
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
            GameObject newCoin = PickupsSpawns.Instance.SpawnPickUp(CoinPrefab, transform.position);
            newCoin.transform.position = transform.position;
            newCoin.GetComponent<Rigidbody2D>().AddForce(Vector2.down * (coinImpulseOnDrop - rb.velocity.y), ForceMode2D.Impulse);
        }

        else if (playerData.showingCoin && nearbyItems.Count == 0)
        {
            nearbyItems.Add(playerData.showedCoin);
            CoinGone(playerData.showedCoin.GetComponent<Coin>());
            //playerData.showedCoin.GetComponent<Rigidbody2D>().AddForce(Vector2.down * coinImpulseOnDrop, ForceMode2D.Impulse);
        }

    }
    public void ShowCoinInput()
    {
        if (coins > 0 && !playerData.showingCoin && !playerData.burningIron)
        {
            //rb.velocity = Vector2.zero;
            GameObject newCoin = PickupsSpawns.Instance.SpawnPickUp(CoinPrefab, playerData.shootPoint.position);

            playerData.showedCoin = newCoin;
            Coin coinScript = newCoin.GetComponent<Coin>();
            CoinInHand(coinScript);

            UpdateCoins(-1);
            showCoinCounter = playerData.showCoinTime;
            playerData.showingCoin = true;
        }
    }
    #endregion

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

            if (showCoinCounter < 0.01f || playerData.damaged)
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
        UpdateMetalReserves();
        UpdateCanvas();
    }
    #region reserves update

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
            UpdateIronReserves(Time.timeScale > 0f ? -Time.unscaledDeltaTime : -0);
        }

        if (!steelEmpty && playerData.burningSteel)
        {
            UpdateSteelReserves(Time.timeScale > 0f ? -Time.unscaledDeltaTime : -0);
        }

        if (!tinEmpty && playerData.burningTin)
        {
            UpdateTinReserves(Time.timeScale > 0f ? -Time.unscaledDeltaTime : -0);
        }

        if (!pewterEmpty && playerData.burningPewter)
        {
            UpdatePewterReserves(Time.timeScale > 0f ? -Time.unscaledDeltaTime : -0);
        }
    }
    void UpdateMetalReserves()
    {
        if (ironReserve < 0.01f && ironVials > 0)
        {
            UpdateIronReserves(60f);
            UpdateIronVials(-1);
            if (ironEmpty) ironEmpty = false;
        }

        if (steelReserve < 0.01f && steelVials > 0)
        {
            UpdateSteelReserves(60f);
            UpdateSteelVials(-1);
            if (steelEmpty) steelEmpty = false;
        }

        if (tinReserve < 0.01f && tinVials > 0)
        {
            UpdateTinReserves(60f);
            UpdateTinVials(-1);
            if (tinEmpty) tinEmpty = false;
        }

        if (pewterReserve < 0.01f && pewterVials > 0)
        {
            UpdatePewterReserves(60f);
            UpdatePewterVials(-1);
            if (pewterEmpty) pewterEmpty = false;
        }
    }
    void PickVialUpdateMetalReserves(Vial vial)
    {
        switch (vial.type)
        {
            case VialType.iron:
                if (ironEmpty)
                {
                    UpdateIronReserves(60f);
                    ironEmpty = false;
                }
                else UpdateIronVials(1);
                break;
            case VialType.steel:
                if (steelEmpty)
                {
                    UpdateSteelReserves(60f);
                    steelEmpty = false;
                }
                else UpdateSteelVials(1);
                break;
            case VialType.tin:
                if (tinEmpty)
                {
                    UpdateTinReserves(60f);
                    tinEmpty = false;
                }
                else UpdateTinVials(1);
                break;
            case VialType.pewter:
                if (pewterEmpty)
                {
                    UpdatePewterReserves(60f);
                    pewterEmpty = false;
                }
                else UpdatePewterVials(1);
                break;
        }
    }
    #endregion

    #region Item Metal Interaction

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
    #endregion

    #region Canvas Updates



    void UpdateCoins(int amount)
    {
        coins += amount;
    }

    void UpdateIronReserves(float amount)
    {
        ironReserve += amount;
        if (ironReserve < 0.01f)
        {
            ironReserve = 0f;
        }
    }

    void UpdateSteelReserves(float amount)
    {
        steelReserve += amount;
        if (steelReserve < 0.01f)
        {
            steelReserve = 0f;
        }
    }
    void UpdateTinReserves(float amount)
    {
        tinReserve += amount;
        if (tinReserve < 0.01f)
        {
            tinReserve = 0f;
        }
    }
    void UpdatePewterReserves(float amount)
    {
        pewterReserve += amount;
        if (pewterReserve < 0.01f)
        {
            pewterReserve = 0f;
        }
    }

    void UpdateIronVials(int amount)
    {
        ironVials += amount;
    }
    void UpdateSteelVials(int amount)
    {
        steelVials += amount;
    }
    void UpdateTinVials(int amount)
    {
        tinVials += amount;
    }
    void UpdatePewterVials(int amount)
    {
        pewterVials += amount;
    }
    
    public void UpdateCanvas()
    {
        UIResources.instance.UpdateCoins(coins);

        UIResources.instance.UpdateVials(VialType.iron, ironVials);
        UIResources.instance.UpdateVials(VialType.steel, steelVials);
        UIResources.instance.UpdateVials(VialType.tin, tinVials);
        UIResources.instance.UpdateVials(VialType.pewter, pewterVials);

        UIResources.instance.UpdateSlider(VialType.iron, ironReserve);
        UIResources.instance.UpdateSlider(VialType.steel, steelReserve);
        UIResources.instance.UpdateSlider(VialType.tin, tinReserve);
        UIResources.instance.UpdateSlider(VialType.pewter, pewterReserve);
    }
    #endregion
}
