using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewterPower : MonoBehaviour
{
    private PlayerData playerData;
    private bool input = false;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
    }
    public void PewterInput()
    {
        if (input) //stop burning
        {
            input = false;
            playerData.burningPewter = false;
            playerData.moveMod = 1;
            playerData.jumpMod = 1;
            playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
        }
        else //start burning
        {
            input = true;
            playerData.burningPewter = true;
            playerData.moveMod = playerData.pewterMovementModifier;
            playerData.jumpMod = playerData.pewterJumpModifier;
            playerData.smearFramesMaterial.SetInt("_burningPewter", 1);

        }
    }
    
}
