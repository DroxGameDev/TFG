using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TinPower : MonoBehaviour
{
    private bool input = false;
    private PlayerData playerData;
    private PlayerResources playerResources;
    private bool transitioning;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerResources = GetComponent<PlayerResources>();
        CameraManager.Instance._currentCamera.m_Lens.FieldOfView = playerData.defaultCameraSize;
        playerData.mist.SetFloat("_FogStrengh", 0f);
        playerData.hiddenWall.SetFloat("_Alpha", 1f);

    }

    public void TinInput()
    {
        if (!transitioning)
        {
            if (input) //stop burning
            {
                input = false;
                playerData.burningTin = false;
                StartCoroutine(EndTransition());
            }
            else //start burning
            {
                input = true;
                playerData.burningTin = true;
                StartCoroutine(StartTransition());
            }
        }
    }

    void Update()
    {
        if (playerData.burningTin && (playerResources.tinEmpty || playerData.dead)){
            input = false;
            playerData.burningTin = false;
            StartCoroutine(EndTransition());
        }
    }

    private IEnumerator StartTransition()
    {
        float currentStep = 0;
        transitioning = true;

        while (currentStep < 1f)
        {
            float currentFogStrengh = Mathf.Lerp(0f, playerData.seeThroughMistSize, currentStep);
            float currenCameraSize = Mathf.Lerp(playerData.defaultCameraSize, playerData.tinCameraSize, currentStep);

            playerData.mist.SetFloat("_FogStrengh", currentFogStrengh);
            CameraManager.Instance._currentCamera.m_Lens.FieldOfView = currenCameraSize;

            float currentWallAlpha = Mathf.Lerp(1f,0f, currentStep);
            playerData.hiddenWall.SetFloat("_Alpha", currentWallAlpha);

            currentStep += playerData.tinTransitionStep;
            yield return new WaitForSeconds(0.01f);
        }

        transitioning = false;
    }
    
    private IEnumerator EndTransition()
    {
        float currentStep = 1;
        transitioning = true;

        while (currentStep > 0f)
        {
            float currentFogStrengh = Mathf.Lerp(0f, playerData.seeThroughMistSize, currentStep); 
            float currenCameraSize = Mathf.Lerp(playerData.defaultCameraSize, playerData.tinCameraSize, currentStep); 

            playerData.mist.SetFloat("_FogStrengh", currentFogStrengh);
            CameraManager.Instance._currentCamera.m_Lens.FieldOfView = currenCameraSize;

            float currentWallAlpha = Mathf.Lerp(1f, 0f, currentStep);
            playerData.hiddenWall.SetFloat("_Alpha", currentWallAlpha);

            currentStep -= playerData.tinTransitionStep;
            yield return new WaitForSeconds(0.01f);
        }

        transitioning = false;
    }
}
