using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Pick : MonoBehaviour
{
    TutorialInfo tutorialInfo;

    void Awake()
    {
        tutorialInfo = GetComponent<TutorialInfo>();
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            tutorialInfo.ShowTutorial();
        }
        if (collision.tag == "Vial")
        {
            Destroy(gameObject);
        }
    }
}
