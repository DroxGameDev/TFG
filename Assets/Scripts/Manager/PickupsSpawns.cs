using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupsSpawns : MonoBehaviour
{
    public static PickupsSpawns Instance { get; private set; }

    public string pickupsScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }   
    }

    public GameObject SpawnPickUp(GameObject pickUp, Vector3 position)
    {
        GameObject pickUpInstance = Instantiate(pickUp, position, quaternion.identity);

        Scene targetScene = SceneManager.GetSceneByName(pickupsScene);

        if (targetScene.IsValid() && targetScene.isLoaded)
        {
            SceneManager.MoveGameObjectToScene(pickUpInstance, targetScene);
        }
        else
        {
            Debug.Log("falta la escena");
        }

        return pickUpInstance;
    }
}
