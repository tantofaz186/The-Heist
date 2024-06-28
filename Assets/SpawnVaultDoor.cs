using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnVaultDoor : NetworkBehaviour
{
    public GameObject vaultDoor;
    public Transform spawnPoint;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            var instance =  Instantiate(vaultDoor, spawnPoint);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
        }
    }
}
