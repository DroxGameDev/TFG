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

    [Header("UI Continers")]
    public GameObject iron;
    public GameObject steel;
    public GameObject tin;
    public GameObject pewter;

    [Header("Reserves Backgrounds")]
    public GameObject ironDefault;
    public GameObject ironFire;

    public GameObject steelDefault;
    public GameObject steelFire;

    public GameObject tinDefault;
    public GameObject tinFire;
    
    public GameObject pewterDefault;
    public GameObject pewterFire;
    

    [Header("Number Vials")]
    public TMP_Text ironText;
    public TMP_Text steelText;
    public TMP_Text tinText;
    public TMP_Text pewterText;

    [Header("Reserves amounts")]
    public Slider ironSlider;
    public Slider steelSlider;
    public Slider tinSlider;
    public Slider pewterSlider;

    private PlayerData playerData;

    void Awake()
    {
        instance = this;

        ironDefault.SetActive(true);
        steelDefault.SetActive(true);
        tinDefault.SetActive(true);
        pewterDefault.SetActive(true);

        ironFire.SetActive(false);
        steelFire.SetActive(false);
        tinFire.SetActive(false);
        pewterFire.SetActive(false);

    }

    void Start()
    {
        playerData = GameManager.Instance.player.GetComponent<PlayerData>();
    }

    void Update()
    {
        FireHandler(ironDefault, ironFire, playerData.burningIron);
        FireHandler(steelDefault, steelFire, playerData.burningSteel);
        FireHandler(tinDefault, tinFire, playerData.burningTin);
        FireHandler(pewterDefault, pewterFire, playerData.burningPewter);
    }

    private void FireHandler(GameObject defalut, GameObject fire, bool burningMetal)
    {
        if (burningMetal)
        {
            defalut.SetActive(false);
            fire.SetActive(true);
        }
        else
        {
            defalut.SetActive(true);
            fire.SetActive(false);
        }
    }


    public void UpdateCoins(int coinsAmount)
    {
        if (coinsAmount == 0)
        {
            coinText.enabled = false;
        }
        else
        {
            coinText.enabled = true;
        }
        coinText.text = coinsAmount.ToString();
    }

    public void UpdateVials(VialType type, int vialAmount)
    {
        switch (type)
        {
            case VialType.iron:
                ironText.text = vialAmount.ToString();
                if (vialAmount == 0)
                    ironText.enabled = false;
                else
                    ironText.enabled = true;
                return;

            case VialType.steel:
                steelText.text = vialAmount.ToString();
                if (vialAmount == 0)
                    steelText.enabled = false;
                else
                    steelText.enabled = true;
                return;

            case VialType.tin:
                tinText.text = vialAmount.ToString();
                if (vialAmount == 0)
                    tinText.enabled = false;
                else
                    tinText.enabled = true;
                return;
            case VialType.pewter:
                pewterText.text = vialAmount.ToString();
                if (vialAmount == 0)
                    pewterText.enabled = false;
                else
                    pewterText.enabled = true;
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
                if(value < 0.1f)
                    iron.SetActive(false);
                else
                    iron.SetActive(true);
                return;

            case VialType.steel:
                steelSlider.value = sliderValue;
                if(value < 0.1f)
                    steel.SetActive(false);
                else
                    steel.SetActive(true);
                return;

            case VialType.tin:
                tinSlider.value = sliderValue;
                if(value < 0.1f)
                    tin.SetActive(false);
                else
                    tin.SetActive(true);
                return;

            case VialType.pewter:
                pewterSlider.value = sliderValue;
                if(value < 0.1f)
                    pewter.SetActive(false);
                else
                    pewter.SetActive(true);
                return;
        }
    }
}
