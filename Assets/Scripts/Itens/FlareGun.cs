using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlareGun : BaseItem
{
    NetworkVariable<bool> used = new NetworkVariable<bool>(false);
    [SerializeField] GameObject flarePrefab;
    public override void UseItem()
    {
        if(!used.Value)
        {
            ShootFlare();
        }
    }
    
    public void ShootFlare()
    {
        used.Value = true;
        InstantiateFlareRpc();
    }


    [Rpc(SendTo.Server)]
   void InstantiateFlareRpc()
    {
        if (IsServer)
        {
            var instance = Instantiate(flarePrefab, transform.position, Quaternion.identity);
            var instanceNetwork = instance.GetComponent<NetworkObject>();
            instanceNetwork.SpawnWithOwnership(OwnerClientId);
        }
    }
}
