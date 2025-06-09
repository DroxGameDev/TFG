using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkableArea : MonoBehaviour
{
    public GravityMode gravity;
    private  Collider2D col;

    public Vector2 worldOffest;

    void Start()
    {
        col = GetComponent<Collider2D>();
        col.offset = Vector2.up;

        if (gravity == GravityMode.Up)
        {
            worldOffest = Vector2.down;
        }
        else if (gravity == GravityMode.Down)
        {
            worldOffest = Vector2.up;
        }
        else if (gravity == GravityMode.Left)
        {
            worldOffest = Vector2.right;
        }
        else if (gravity == GravityMode.Right)
        {
            worldOffest = Vector2.left;
        }
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
