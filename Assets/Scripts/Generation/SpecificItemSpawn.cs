using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpecificItemSpawn : NetworkBehaviour
{

    
   [SerializeField]public GameObject relic;

    [SerializeField]private List<GameObject> itemSpawnPoints = new();

    [Rpc(SendTo.Server)]
   public void SpawnItemsRpc()
   {
      for(int i = 0; i < itemSpawnPoints.Count; i++)
      {
         SpawnItems(i);
      }
      

   }

   void SpawnItems( int spawnPointIndex)
   {
            var instance = Instantiate(relic,
            itemSpawnPoints[spawnPointIndex].transform);
         var instanceNetworkObject = instance.GetComponent<NetworkObject>();
         instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
     
   }

   void Start(){
      SpawnItemsRpc();
   }
}
