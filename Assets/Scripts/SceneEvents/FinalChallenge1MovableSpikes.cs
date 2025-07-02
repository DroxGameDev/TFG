using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChallenge1MovableSpikes : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Close", true);
        }
    }
}
