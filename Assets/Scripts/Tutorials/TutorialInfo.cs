using UnityEngine;

public class TutorialInfo : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject gamePadInput;
    public GameObject keyboardInput;

    public void Awake()
    {
        tutorialPanel.SetActive(false);
        gamePadInput.SetActive(true);
        keyboardInput.SetActive(false);
    }

    void Update()
    {
        if (Tutorials.Instance.controllerState == ControllerState.GamePad)
        {

            if (gamePadInput.activeSelf == false)
            {
                gamePadInput.SetActive(true);
                keyboardInput.SetActive(false);
            }

        }
        else if (Tutorials.Instance.controllerState == ControllerState.Keyboard)
        {
            if (keyboardInput.activeSelf == false)
            {
                keyboardInput.SetActive(true);
                gamePadInput.SetActive(false);
            }
        }
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

}
