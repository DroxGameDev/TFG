using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Move : MonoBehaviour
{
    TutorialInfo tutorialInfo;
    public GameObject initialCanvas;

    void Awake()
    {
        tutorialInfo = GetComponent<TutorialInfo>();
    }

    void Update()
    {
        if (MenuManager.Instance.gameStarted)
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
