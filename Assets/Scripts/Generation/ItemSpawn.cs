using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.VaultDoor;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawn : NetworkBehaviour
{
   [SerializeField]public List<GameObject> items = new();
   [SerializeField]private List<GameObject> itemSpawnPoints = new();
   [SerializeField] private List<int> itemsCheck = new();
   
   [Rpc(SendTo.Server)]
   public void SpawnItemsRpc()
   {
      itemSpawnPoints = GetItemSpawnPoints();
      for (int i = 0; i < 4; i++)
      {
         SortItemsCodigo(i);
      }
      for (int n = 4; n < itemSpawnPoints.Count; n++)
      {
         SortItems(n);
      }

   }
   void SortItemsCodigo(int x)
   {

      var instance = Instantiate(items.First(item => item.TryGetComponent<CodigoSpawnItem>(out _)),
            itemSpawnPoints[x].transform);
         var instanceNetworkObject = instance.GetComponent<NetworkObject>();
         instanceNetworkObject.SpawnWithOwnership(OwnerClientId);

   }
   void SortItems(int x)
   {
      int rnd = Random.Range(0, items.Count);
      bool inList = false;
      if (itemsCheck.Count <= 0)
      {
         itemsCheck = new List<int>(){0,1,2,3,4,5};
          
      }
           
       
      for (int i=0;i<itemsCheck.Count;i++)
      {   
         if (rnd == itemsCheck[i])
         {   
            inList = true;
         }  
      }

      if (inList)
      {    
         itemsCheck.Remove(rnd);
           
        
         var instance = Instantiate(items[rnd],
            itemSpawnPoints[x].transform);
         var instanceNetworkObject = instance.GetComponent<NetworkObject>();
         instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
      }
      else
      {
         SortItems(x);
      }
   }
   
   List<GameObject> GetItemSpawnPoints()
   {
      return GameObject.FindGameObjectsWithTag("ItemSpawnPoints").ToList();
      
   }
   
}
