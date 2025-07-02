using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronChallengeMovableDoor : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            anim.SetBool("Open", true);
        }
    }
}
