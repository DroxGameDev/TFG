using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIResources : MonoBehaviour
{
    public static UIResources instance { get; set; }
    [Header("Coins")]
    public TMP_Text coinText;

    [Header("Vials")]
    public TMP_Text ironText;
    public TMP_Text steelText;
    public TMP_Text tinText;
    public TMP_Text pewterText;

    [Header("Reserves")]
    public Slider ironSlider;
    public Slider steelSlider;
    public Slider tinSlider;
    public Slider pewterSlider;

    void Awake()
    {
        instance = this;
    }

    public void UpdateCoins(int coinsAmount)
    {
        coinText.text = coinsAmount.ToString();
    }

    public void UpdateVials(VialType type, int vialAmount)
    {
        switch (type)
        {
            case VialType.iron:
                ironText.text = vialAmount.ToString();
                return;
            case VialType.steel:
                steelText.text = vialAmount.ToString();
                return;
            case VialType.tin:
                tinText.text = vialAmount.ToString();
                return;
            case VialType.pewter:
                pewterText.text = vialAmount.ToString();
                return;
        }
    }

    public void UpdateSlider(VialType type, float value)
    {
        float sliderValue = Mathf.InverseLerp(0f, 60f, value);
        switch (type)
        {
            case VialType.iron:
                ironSlider.value = sliderValue;
                return;
            case VialType.steel:
                steelSlider.value = sliderValue;
                return;
            case VialType.tin:
                tinSlider.value = sliderValue;
                return;
            case VialType.pewter:
                pewterSlider.value = sliderValue;
                return;
        }
    }
}
