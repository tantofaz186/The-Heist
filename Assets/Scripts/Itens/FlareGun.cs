using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlareGun : BaseItem
{
    NetworkVariable<int> bullets = new NetworkVariable<int>(0);
    [SerializeField] GameObject flarePrefab;
    [SerializeField] Transform bulletSpawn;
    public override void UseItem()
    {
        if(bullets.Value<=4)
        {
            ShootFlare();
        }
    }
    
    public void ShootFlare()
    {
        
        InstantiateFlareRpc();
    }


    [Rpc(SendTo.Server)]
   void InstantiateFlareRpc()
    {
            bullets.Value++;
            var instance = Instantiate(flarePrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            var instanceNetwork = instance.GetComponent<NetworkObject>();
            instanceNetwork.SpawnWithOwnership(OwnerClientId);
            Rigidbody rb =instance.GetComponent<Rigidbody>();
            rb.AddForce(rb.transform.forward*30,ForceMode.Impulse);
            
    }
}
