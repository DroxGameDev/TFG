using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChallenge1Movabledoor : MonoBehaviour
{
    private Animator anim;

    private Collider2D col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Heavy_Metal"))
        {
            anim.SetBool("Open", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Heavy_Metal"))
        {
            anim.SetBool("Close", true);
        }
    }
}
