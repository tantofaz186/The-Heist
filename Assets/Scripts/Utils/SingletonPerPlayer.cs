using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//só derivar o objeto Singleton dessa classe, ou de SingletonPersistent se não for pra destruir on load
namespace Utils
{
    [DefaultExecutionOrder(-1)]
    public abstract class SingletonPerPlayer<T> : NetworkBehaviour
        where T : Component
    {
        public static T Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            NetworkClient client = NetworkManager.Singleton.LocalClient;
            Instance = client.PlayerObject.GetComponent<T>();
        }

        // [Rpc(SendTo.Everyone, RequireOwnership = false)]
        // private void SendToClientRpc()
        // {
        //     NetworkClient client = NetworkManager.Singleton.LocalClient;
        //     Instance = client.PlayerObject.GetComponent<T>();
        //     Ready = true;
        // }
    }
}