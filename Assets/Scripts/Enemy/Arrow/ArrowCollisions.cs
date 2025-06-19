using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollisions : MonoBehaviour
{
    public Arrow origin;

    void OnTriggerEnter2D(Collider2D collision)
    {
        origin.Collision(collision);
    }
}
