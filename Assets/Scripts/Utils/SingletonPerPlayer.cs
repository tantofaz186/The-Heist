using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

//só derivar o objeto Singleton dessa classe, ou de SingletonPersistent se não for pra destruir on load
/*[DefaultExecutionOrder(-1)]*/
[RequireComponent(typeof(ClientNetworkTransform))]
[RequireComponent(typeof(PlayersManager))]
public class SingletonPerPlayer<T> : NetworkBehaviour
    where T : Component
{
    public static Dictionary<ulong, SingletonPerPlayer<T>> Players = new Dictionary<ulong, SingletonPerPlayer<T>>();

    public static T Instance => Players.TryGetValue(NetworkManager.Singleton.LocalClientId, out SingletonPerPlayer<T> player) ? player.GetComponent<T>() : null;

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