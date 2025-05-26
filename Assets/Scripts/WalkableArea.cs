using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkableArea : MonoBehaviour
{
    public GravityMode gravity;
    private Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
        if (gravity == GravityMode.Up)
        {
            col.offset = Vector2.down;
        }
        else if (gravity == GravityMode.Left)
        {
            col.offset = Vector2.right;
        }
        else if (gravity == GravityMode.Right)
        {
            col.offset = Vector2.left;
        }
        else
            col.offset = Vector2.up;

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<IronPower>();
        
        if (player != null)
            player.SetWalkableArea(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<IronPower>();

        if (player != null)
            player.ResetWalkableArea(this);
    }
}
