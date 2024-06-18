using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

//só derivar o objeto Singleton dessa classe, ou de SingletonPersistent se não for pra destruir on load
/*[DefaultExecutionOrder(-1)]*/
public abstract class SingletonPerPlayer<T> : NetworkBehaviour
    where T : Component
{
    public static Dictionary<ulong, T> Players = new Dictionary<ulong, T>();
    public static T Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {        
            foreach (var keyValuePair in NetworkManager.ConnectedClients)
            {
                Players[keyValuePair.Key] = keyValuePair.Value.PlayerObject.GetComponent<T>();
                
            }
            SendToClientRpc();
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void SendToClientRpc()
    {
        NetworkClient client = NetworkManager.Singleton.LocalClient;
        Instance = client.PlayerObject.GetComponent<T>();
    }
}