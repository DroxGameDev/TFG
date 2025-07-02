using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Door : MonoBehaviour
{
    public Door destinyDoor;
    public SceneInfo doorScene;
    public CinemachineVirtualCamera doorCamera;

    public RespawnPlayerInfo respawnInfo;

    public bool isCheckipoint = true;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerData>().nearbyDoor = this;
            if(isCheckipoint)
                GameManager.Instance.UpdateCheckpoint(transform, doorScene, doorCamera, respawnInfo);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
                collision.GetComponent<PlayerData>().nearbyDoor = null;
    }
}
