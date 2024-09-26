using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public EventHandler OnReadyChanged;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    
    public void SetPlayerReady()
    { 
        SetPlayerReadyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            TheHeistGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }
    
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
