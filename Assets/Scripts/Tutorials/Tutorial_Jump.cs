using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Jump : MonoBehaviour
{
   TutorialInfo tutorialInfo;

    void Awake()
    {
        tutorialInfo = GetComponent<TutorialInfo>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            tutorialInfo.ShowTutorial();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
