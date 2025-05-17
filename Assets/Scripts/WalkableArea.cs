using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkableArea : MonoBehaviour
{
    public GravityMode gravity;

    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<IronPower2>();
        
        if (player != null)
            player.SetWalkableArea(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<IronPower2>();

        if (player != null)
            player.ResetWalkableArea(this);
    }
}
