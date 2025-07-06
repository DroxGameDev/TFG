using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinPewterTutorialMovableSpikes : MonoBehaviour
{
    SpikesChase spikeChase;

    bool chaseDone = false;

    void Awake()
    {
        spikeChase = GetComponent<SpikesChase>();   
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !chaseDone)
        {
            spikeChase.BeginChase();
            chaseDone = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Box")
        {
            spikeChase.ChaseReturn();
        }
    }
}
