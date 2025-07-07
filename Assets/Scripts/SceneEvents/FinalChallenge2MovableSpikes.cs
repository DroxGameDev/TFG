using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChallenge2MovableSpikes : MonoBehaviour
{
    Animator anim;
    Collider2D col;
    public LayerMask enemyLayer;

    SpikesChase spikesChase;
    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        spikesChase = GetComponent<SpikesChase>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (!col.IsTouchingLayers(enemyLayer))
            {
                anim.SetTrigger("Open");
                spikesChase.BeginChase();
            }
        }
    }
}
