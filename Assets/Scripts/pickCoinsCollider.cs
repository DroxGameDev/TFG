using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickCoinsCollider : MonoBehaviour
{
    public PlayerResources playerResources;

    void OnTriggerEnter2D(Collider2D collision)
    {
        playerResources.GetCoin(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        playerResources.RemoveCoin(collision);
    }
    
}
