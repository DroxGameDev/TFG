using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    [Header("Menus")]
    [SerializeField] private GameObject beginMenu;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private GameObject resuourcesInfo;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header ("Player Components")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("First Selected Opstions")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject settingsMenuFirst;
    [SerializeField] private GameObject beginMenuFirst;



    public bool isPaused = false;
    private void Start()
    {
        beginMenu.SetActive(true);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        endMenu.SetActive(false);
        resuourcesInfo.SetActive(false);

        EventSystem.current.SetSelectedGameObject(beginMenuFirst);
    }

    public void PauseMenuInput()
    {

        if (!isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }
    #region Begin and end Menu

    public void BeginButtonPressed()
    {
        beginMenu.SetActive(false);
        resuourcesInfo.SetActive(true);
        GameManager.Instance.StartGame();
    }

    public void EndMenuTriggered()
    {
        endMenu.SetActive(true);
        resuourcesInfo.SetActive(false);

    }

    #endregion

    #region Pause/Unpause
    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        OpenMainMenu();
    }

    void Unpause()
    {
        isPaused = false;
        if (Iron_Steel.state == PowerState.select)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        playerMovement.MoveInputUpdate(Vector2.zero);

        CloseAllMenus();
    }

    #endregion

    #region Canvas activaton

    void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(settingsMenuFirst);
    }

    void CloseAllMenus()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }
    #endregion

    #region main menu buttons actions

    public void OnLastcheckpointPressed()
    {
        playerMovement.GetComponent<PlayerDie>().Die();
        Unpause();
    }

    public void OnSettingsPressed()
    {
        OpenSettingsMenu();
    }

    public void OnResumePressed()
    {
        Unpause();
    }

    public void OnExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }

    #endregion

    #region Settings menu buttons actions
    public void OnSettingsBackPressed()
    {
        OpenMainMenu();
    }



    #endregion
}
