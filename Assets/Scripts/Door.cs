using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door destinyDoor;

    public SceneInfo doorScene;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerData>().nearbyDoor = this;
            GameManager.Instance.UpdateCheckpoint(transform, doorScene);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
                collision.GetComponent<PlayerData>().nearbyDoor = null;
    }
}
