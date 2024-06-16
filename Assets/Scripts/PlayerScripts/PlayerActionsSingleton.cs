using System;
using System.Collections;
using UnityEngine;

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

    public void OnDisable()
    {
        playerInputActions.Disable();
    }
}
