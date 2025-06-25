using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Door destinyDoor;

    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<PlayerData>().nearbyDoor = this;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<PlayerData>().nearbyDoor = null;
    }
}
