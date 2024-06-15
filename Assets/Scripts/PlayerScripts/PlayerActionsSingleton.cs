using System;
using System.Collections;
using UnityEngine;

public class PlayerActionsSingleton : SingletonPerPlayer<PlayerActionsSingleton>
{
    private PlayerInputActions playerInputActions;

    public PlayerInputActions PlayerInputActions => playerInputActions;
    public Camera _camera;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Enable();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerInputActions.Disable();
    }
}
