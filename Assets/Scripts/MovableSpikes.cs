using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableSpikes : MonoBehaviour
{
    public Animator anim;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            anim.SetBool("Close", true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Heavy_Metal")
        {
            anim.SetBool("Open", true);
        }
    }
}
