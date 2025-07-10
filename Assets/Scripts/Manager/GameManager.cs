using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform player;

    public float RespawnWait;

    //public List<string> scenesToReload;
    public Transform respawnPoint;
    public RespawnPlayerInfo respawnPlayerInfo;
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
    }

    void Start()
    {
        player.GetComponent<PlayerInput>().actions.Disable();
    }
    public void StartGame()
    {
        player.GetComponent<PlayerInput>().actions.Enable();
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
        if (originDoor.destinyDoor != null)
            StartCoroutine(MoveToDoor(originDoor.destinyDoor));
    }

    IEnumerator MoveToDoor(Door destinyDoor)
    {
        player.GetComponent<PlayerInput>().actions.Disable();

        yield return StartCoroutine(FadeBlackOutSquare(true));

        CameraManager.Instance.ChangeCamera(destinyDoor.doorCamera);

        ViewManager.Instance.ChangeScene(destinyDoor.doorScene);

        while (ViewManager.Instance.changingScene || cinemachineBrain.IsBlending)
        {
            // Wait until the scene is changed and the camera is done blending
            yield return null;
        }
        //CameraManager.Instance.ChangeCameraBoundaries(destinyDoor.doorScene.cameraBounds);
        LoadPlayerInfo(destinyDoor.respawnInfo);
        player.GetComponent<PlayerResources>().UpdateCanvas();

        player.transform.position = destinyDoor.transform.position;

        yield return new WaitForSeconds(fadeWait);

        yield return StartCoroutine(FadeBlackOutSquare(false));

        player.GetComponent<PlayerInput>().actions.Enable();
    }
    public void UpdateCheckpoint(Transform checkPointTransport, SceneInfo scene, CinemachineVirtualCamera camera, RespawnPlayerInfo respawnInfo)
    {
        if (respawnPoint != checkPointTransport)
        {
            respawnPoint = checkPointTransport;
            respawnPlayerInfo = respawnInfo;
            ViewManager.Instance.checkPointScene = scene;
            ViewManager.Instance.currentScene = scene;

            CameraManager.Instance.respawnCamera = camera;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(RespawnWait);
        yield return StartCoroutine(FadeBlackOutSquare(true));

        ViewManager.Instance.Respawn();
        CameraManager.Instance.Respawn();

        player.transform.position = respawnPoint.position;
        LoadPlayerInfo(respawnPlayerInfo);
        player.GetComponent<PlayerResources>().UpdateCanvas();

        yield return new WaitForSeconds(fadeWait);

        player.GetComponent<PlayerDie>().RespawnValues();
        Time.timeScale = 1f;

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

    private void LoadPlayerInfo(RespawnPlayerInfo playerInfo)
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
