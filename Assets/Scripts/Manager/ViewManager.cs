using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    public SceneInfo beginScene;
    public SceneInfo currentScene;
    public SceneInfo checkPointScene;
    public bool changingScene = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (!SceneManager.GetSceneByName(beginScene.sceneName).isLoaded)
        {
            SceneManager.LoadSceneAsync(beginScene.sceneName, LoadSceneMode.Additive);
        }
    }

    public void ChangeScene(SceneInfo scene)
    {
        StartCoroutine(ChangeSceneRutine(scene));
    }

    IEnumerator ChangeSceneRutine(SceneInfo scene)
    {
        changingScene = true;

        SceneManager.UnloadSceneAsync(currentScene.sceneName);

        if (!SceneManager.GetSceneByName(scene.sceneName).isLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
            yield return loadOperation;
        }

        currentScene = scene;

        changingScene = false;
    }

    public void Respawn()
    {
        SceneManager.UnloadSceneAsync(currentScene.sceneName);

        SceneManager.LoadSceneAsync(checkPointScene.sceneName, LoadSceneMode.Additive);

    }

    public void Restart()
    {
        SceneManager.UnloadSceneAsync(currentScene.sceneName);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadSceneAsync("SampleScene");
    }

}
