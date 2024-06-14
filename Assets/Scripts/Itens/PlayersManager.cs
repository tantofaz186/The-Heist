using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayersManager : NetworkBehaviour
{
    
    public static Dictionary<ulong, PlayersManager> Players = new Dictionary<ulong, PlayersManager>();
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
