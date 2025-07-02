using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialInfo : MonoBehaviour
{
    public string GamePadTutorial;
    public string KeyboardTutorial;
    string currentTutorial; 
    TextMeshPro textMeshPro;

    void Awake()
    {
        currentTutorial = GamePadTutorial;
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = currentTutorial;
    }


    void Update()
    {
        if(Tutorials.Instance.controllerState == ControllerState.GamePad)
        {
            if (currentTutorial != GamePadTutorial)
            {
                currentTutorial = GamePadTutorial;
                textMeshPro.text = currentTutorial;
            }
        }
        else if (Tutorials.Instance.controllerState == ControllerState.Keyboard)
        {
            if (currentTutorial != KeyboardTutorial)
            {
                currentTutorial = KeyboardTutorial;
                textMeshPro.text = currentTutorial;
            }
        }
    }

}
