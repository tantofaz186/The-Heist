using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerActionsSingleton : Singleton<PlayerActionsSingleton>
{
    private PlayerInputActions playerInputActions;

    public PlayerInputActions PlayerInputActions => playerInputActions;
    public Camera _camera;
    private void OnEnable()
    {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
