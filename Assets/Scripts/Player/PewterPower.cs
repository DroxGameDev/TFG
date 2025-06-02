using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewterPower : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerResources playerResources;
    private AttackInfo attackInfo;
    private bool input = false;

    private int noPewterDamage;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerResources = GetComponent<PlayerResources>();
        attackInfo = playerData.attackOrigin.GetComponent<AttackInfo>();

        noPewterDamage = playerData.damage;
        attackInfo.burningPewter = false;
        playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
    }
    public void PewterInput()
    {
        if (input) //stop burning
        {
            input = false;
            playerData.burningPewter = false;
            attackInfo.burningPewter = false;
            playerData.moveMod = 1;
            playerData.jumpMod = 1;
            playerData.damage = noPewterDamage;
            playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
        }
        else //start burning
        {
            input = true;
            playerData.burningPewter = true;
            attackInfo.burningPewter = true;
            playerData.moveMod = playerData.pewterMovementModifier;
            playerData.jumpMod = playerData.pewterJumpModifier;
            playerData.damage = playerData.pewterDamage;
            playerData.smearFramesMaterial.SetInt("_burningPewter", 1);

        }
    }

    void Update()
    {
        if (playerData.burningPewter && playerResources.pewterEmpty)
        {
            input = false;
            playerData.burningPewter = false;
            attackInfo.burningPewter = false;
            playerData.moveMod = 1;
            playerData.jumpMod = 1;
            playerData.damage = noPewterDamage;
            playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
        }
    }

}
