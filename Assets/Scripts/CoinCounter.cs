using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    public static CoinCounter instance { get; set; }
    public TMP_Text coinText;
    public int coins = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        coinText.text = coins.ToString();
    }

    public void UpdateCoins(int coinsAmount)
    {
        coinText.text = coinsAmount.ToString();
    }
}
