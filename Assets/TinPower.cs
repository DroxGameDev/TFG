using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinPower : MonoBehaviour
{
    private bool input;
    private PlayerData playerData;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerData.virtualCamera.m_Lens.FieldOfView = playerData.cameraSize;
        playerData.mist.material.SetFloat("_FogStrengh", 0f);
    }

    public void TinInput()
    {
        if (input) //stop burning
        {
            input = false;
            playerData.burningTin = false;
            playerData.mist.material.SetFloat("_FogStrengh", 0f);
            playerData.virtualCamera.m_Lens.FieldOfView = playerData.cameraSize;
        }
        else //start burning
        {
            input = true;
            playerData.burningTin = true;
            playerData.mist.material.SetFloat("_FogStrengh", playerData.seeThroughMistSize);
            playerData.virtualCamera.m_Lens.FieldOfView = playerData.tinCameraSize;

        } 
    }
}
