using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Door : MonoBehaviour
{
    public Door destinyDoor;
    public SceneInfo doorScene;
    public CinemachineVirtualCamera doorCamera;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerData>().nearbyDoor = this;
            GameManager.Instance.UpdateCheckpoint(transform, doorScene, doorCamera);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
                collision.GetComponent<PlayerData>().nearbyDoor = null;
    }
}
