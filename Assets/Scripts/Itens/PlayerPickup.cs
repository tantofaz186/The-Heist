using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Movement))]
public class PlayerPickup : NetworkBehaviour
{

    public static Dictionary<ulong, PlayerPickup> Players = new Dictionary<ulong, PlayerPickup>();
    private PlayerInputActions controle_player;
    public override void OnNetworkSpawn()
    {
        Players[OwnerClientId] = this;

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
{
    if (Players.ContainsKey(OwnerClientId))
    {
        Players.Remove(OwnerClientId);
    }
    base.OnNetworkDespawn();
}
}
