using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerSavedInfo : ScriptableObject
{
    public int health = 0;
    public int coins = 0;
    public int ironVials = 0;
    public int steelVials  = 0;
    public int tinVials= 0;
    public int pewterVials= 0;

    
    public float ironReserve= 0;
    public float steelReserve= 0;
    public float tinReserve= 0;
    public float pewterReserve= 0;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform player;

    private PlayerSavedInfo playerInfo;

    public float RespawnWait;

    //public List<string> scenesToReload;
    public Transform respawnPoint;
    public GameObject CanvasBlackFase;

    public CinemachineBrain cinemachineBrain;
    public float fadeSpeed;
    public float fadeWait;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInfo = ScriptableObject.CreateInstance<PlayerSavedInfo>();
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
        StartCoroutine(MoveToDoor(originDoor.destinyDoor));
    }

    IEnumerator MoveToDoor(Door destinyDoor)
    {
        player.GetComponent<PlayerInput>().actions.Disable();

        yield return StartCoroutine(FadeBlackOutSquare(true));

        CameraManager.Instance.ChangeCamera(destinyDoor.doorCamera);

        ViewManager.Instance.ChangeScene(destinyDoor.doorScene);

        while (ViewManager.Instance.changingScene && cinemachineBrain.IsBlending)
        {
            // Wait until the scene is changed and the camera is done blending
            yield return null;
        }
        //CameraManager.Instance.ChangeCameraBoundaries(destinyDoor.doorScene.cameraBounds);

        player.transform.position = destinyDoor.transform.position;

        yield return new WaitForSeconds(fadeWait);

        yield return StartCoroutine(FadeBlackOutSquare(false));

        player.GetComponent<PlayerInput>().actions.Enable();
    }
    public void UpdateCheckpoint(Transform checkPointTransport, SceneInfo scene, CinemachineVirtualCamera camera)
    {
        if (respawnPoint != checkPointTransport)
        {
            respawnPoint = checkPointTransport;
            ViewManager.Instance.checkPointScene = scene;
            ViewManager.Instance.currentScene = scene;

            CameraManager.Instance.respawnCamera = camera;

            SavePlayerInfo();
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(RespawnWait);
        yield return StartCoroutine(FadeBlackOutSquare(true));

        ViewManager.Instance.Respawn();
        CameraManager.Instance.Respawn();

        player.transform.position = respawnPoint.position;
        LoadPlayerInfo();
        player.GetComponent<PlayerResources>().UpdateCanvas();

        yield return new WaitForSeconds(fadeWait);

        player.GetComponent<PlayerDie>().RespawnValues();

        yield return StartCoroutine(FadeBlackOutSquare(false));
    }

    IEnumerator FadeBlackOutSquare(bool fadeToBlack)
    {
        Color objectColor = CanvasBlackFase.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (CanvasBlackFase.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                CanvasBlackFase.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            while (CanvasBlackFase.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                CanvasBlackFase.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }

    private void SavePlayerInfo()
    {
        PlayerResources playerResources = player.GetComponent<PlayerResources>();

        playerInfo.health = playerResources.health;
        playerInfo.coins = playerResources.coins;
        playerInfo.ironVials = playerResources.ironVials;
        playerInfo.steelVials = playerResources.steelVials;
        playerInfo.tinVials = playerResources.tinVials;
        playerInfo.pewterVials = playerResources.pewterVials;
        playerInfo.ironReserve = playerResources.ironReserve;
        playerInfo.steelReserve = playerResources.steelReserve;
        playerInfo.tinReserve = playerResources.tinReserve;
        playerInfo.pewterReserve = playerResources.pewterReserve;
    }

    private void LoadPlayerInfo()
    {
        PlayerResources playerResources = player.GetComponent<PlayerResources>();

        playerResources.health = playerInfo.health;
        playerResources.coins = playerInfo.coins;
        playerResources.ironVials = playerInfo.ironVials;
        playerResources.steelVials = playerInfo.steelVials;
        playerResources.tinVials = playerInfo.tinVials;
        playerResources.pewterVials = playerInfo.pewterVials;
        playerResources.ironReserve = playerInfo.ironReserve;
        playerResources.steelReserve = playerInfo.steelReserve;
        playerResources.tinReserve = playerInfo.tinReserve;
        playerResources.pewterReserve = playerInfo.pewterReserve;
    }


}
