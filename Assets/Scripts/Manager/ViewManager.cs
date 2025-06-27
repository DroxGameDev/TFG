using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    public string currentScene;
    public string checkPointScene;
    public bool changingScene = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ChangeScene(String scene)
    {
        StartCoroutine(ChangeSceneRutine(scene));
    }

    IEnumerator ChangeSceneRutine(String scene)
    {
        changingScene = true;

        SceneManager.UnloadSceneAsync(currentScene);

        if (!SceneManager.GetSceneByName(scene).isLoaded)
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }
        yield return null;

        currentScene = scene;

        changingScene = false;
    }

    public void Respawn()
    {
        SceneManager.UnloadSceneAsync(currentScene);

        SceneManager.LoadSceneAsync(checkPointScene, LoadSceneMode.Additive);

    }
}
