using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickObjectsCollider : MonoBehaviour
{
    public PlayerResources playerResources;

    void OnTriggerEnter2D(Collider2D collision)
    {
        playerResources.GetObject(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        playerResources.RemoveObject(collision);
    }
    
}
