using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChallenge2MovableSpikes : MonoBehaviour
{
    Animator anim;
    Collider2D col;
    public LayerMask enemyLayer;
    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (!col.IsTouchingLayers(enemyLayer))
            {
                anim.SetTrigger("Open");
            }
        }
    }
}
