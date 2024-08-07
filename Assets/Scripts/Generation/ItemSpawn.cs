using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics.VaultDoor;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawn : NetworkBehaviour
{
   [SerializeField]public List<GameObject> items = new();
   [SerializeField]public List<GameObject> relics = new();
   [SerializeField] private List<GameObject> itemSpawnPoints;
   [SerializeField] private List<int> itemsCheck = new();
   [SerializeField] private List<int> relicsCheck = new();
   [SerializeField] private List<GameObject>codigo;
   
   [Rpc(SendTo.Server)]
   public void SpawnItemsRpc()
   {
      
      
      itemSpawnPoints = GetItemSpawnPoints();
      for(int i = 0; i < itemSpawnPoints.Count; i++)
      {
          int type = itemSpawnPoints[i].GetComponent<itemSpawnType>().spawnType;
          SpawnItems(type, i);
        
      }
   }
  
   
   
   
   void SpawnItems(int type, int spawnPointIndex)
   {
      switch (type)
      {
         case 0:
            SortItems(spawnPointIndex);
            break;
         case 1:
            SortRelics(spawnPointIndex);
            break;
         case 2:
            SortItemsCodigo(spawnPointIndex);
            break;
      }
   }
   
   
   void SortItemsCodigo(int x)
   {
      int rnd = Random.Range(0, codigo.Count);
      var instance = Instantiate(codigo[rnd],itemSpawnPoints[x].transform);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);

   }
   void SortItems(int x)
   {
      int rnd = Random.Range(0, items.Count);
      bool inList = false;
      if (itemsCheck.Count <= 0)
      {
         for (int i = 0; i < items.Count; i++)
         {
            itemsCheck.Add(i);
         }
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
   void SortRelics(int x)
   {
      int rnd = Random.Range(0, relics.Count);
      bool inList = false;
      if (relicsCheck.Count <= 0)
      {
           for (int i = 0; i < relics.Count; i++)
           {
               relicsCheck.Add(i);
           }
      }

     
           
       
      for (int i=0;i<relicsCheck.Count;i++)
      {   
         if (rnd == relicsCheck[i])
         {   
            inList = true;
         }  
      }

      if (inList)
      {    
         relicsCheck.Remove(rnd);
           
        
         var instance = Instantiate(relics[rnd],
            itemSpawnPoints[x].transform);
         var instanceNetworkObject = instance.GetComponent<NetworkObject>();
         instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
      }
      else
      {
         SortRelics(x);
      }
   }

   
   public List<GameObject> GetItemSpawnPoints()
   {
      return GameObject.FindGameObjectsWithTag("ItemSpawnPoints").ToList();
      
   }
   
}
