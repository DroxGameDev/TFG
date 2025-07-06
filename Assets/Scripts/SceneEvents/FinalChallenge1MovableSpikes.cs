using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChallenge1MovableSpikes : MonoBehaviour
{
    SpikesChase spikesChase;

    void Awake()
    {
        spikesChase = GetComponent<SpikesChase>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spikesChase.BeginChase();
        }
    }
}
