using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform player;

    public float RespawnWait;

    public List<string> scenesToReload;
    public Transform respawnPoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
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

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(RespawnWait);

        foreach (string sceneName in scenesToReload)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        yield return null;

        // Reload scenes
        foreach (string sceneName in scenesToReload)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        player.GetComponent<PlayerDie>().RespawnValues();
        player.transform.position = respawnPoint.position;
    }
}
