using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControllerState
{
    GamePad,
    Keyboard
}
public class Tutorials : MonoBehaviour
{
    public static Tutorials Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public ControllerState controllerState;

    public PlayerInput playerController;

    void Update()
    {
        switch (controllerState)
        {
            case ControllerState.GamePad:
                if (playerController.currentControlScheme == "PC")
                    controllerState = ControllerState.Keyboard;
                break;

            case ControllerState.Keyboard:
                if (playerController.currentControlScheme == "GamePad")
                    controllerState = ControllerState.GamePad;
                break;
        }
    }
}
