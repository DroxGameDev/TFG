using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObject : MonoBehaviour
{
    private Collider2D col;
    public PlayerData playerData;

    void OnTriggerEnter2D(Collider2D collision)
    {
        playerData.objectNearby = true;
        playerData.objectToPush = collision.gameObject;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        playerData.objectNearby = false;
        playerData.objectToPush = null;
    } 


}
