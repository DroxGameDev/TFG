using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CheckPoint : MonoBehaviour
{
    public Transform RespawnPoint;
    public SceneInfo Scene;
    public CinemachineVirtualCamera checkPointCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.UpdateCheckpoint(RespawnPoint, Scene, checkPointCamera);
    }
}
