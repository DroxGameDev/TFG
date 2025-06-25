using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform player;

    public float RespawnWait;

    public List<string> scenesToReload;
    public Transform respawnPoint;

    public GameObject CanvasBlackFase;
    public float fadeSpeed;
    public float fadeWait;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterPlayer(Transform playerObject)
    {
        player = playerObject;
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(Respawn());
    }

    public void PlayerToDoor(Door originDoor)
    {
        StartCoroutine(MoveToDoor(originDoor.destinyDoor.transform.position));
    }

    IEnumerator MoveToDoor(Vector3 position)
    {
        yield return StartCoroutine(FadeBlackOutSquare(true));

        player.transform.position = position;
        yield return new WaitForSeconds(fadeWait);

        yield return StartCoroutine(FadeBlackOutSquare(false));
    }
    public void UpdateCheckpoint(Transform checkPointTransport)
    {
        respawnPoint = checkPointTransport;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(RespawnWait);
        yield return StartCoroutine(FadeBlackOutSquare(true));

        foreach (string sceneName in scenesToReload)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        // Reload scenes
        foreach (string sceneName in scenesToReload)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }


        player.transform.position = respawnPoint.position;

        yield return new WaitForSeconds(fadeWait);
        
        player.GetComponent<PlayerDie>().RespawnValues();
        yield return StartCoroutine(FadeBlackOutSquare(false));
    }

    IEnumerator FadeBlackOutSquare(bool fadeToBlack){
        Color objectColor = CanvasBlackFase.GetComponent<Image>().color;
        float fadeAmount;

        if(fadeToBlack)
        {
            while(CanvasBlackFase.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a+ (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                CanvasBlackFase.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            while(CanvasBlackFase.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                CanvasBlackFase.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }
}
