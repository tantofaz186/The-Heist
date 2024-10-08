using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawn : NetworkBehaviour
{
   [SerializeField]public List<GameObject> items = new();
   [SerializeField]public List<GameObject> relics = new();
   [SerializeField] private List<GameObject> itemSpawnPoints;
   [SerializeField] private List<int> itemsCheck = new();
   [SerializeField] private List<int> relicsCheck = new();
   [SerializeField] private GameObject codigo;
   [SerializeField] private List<GameObject> quadros;
[SerializeField] private List<GameObject> rings;
[SerializeField] private List<GameObject> necklaces;
[SerializeField] private List<GameObject> pans;
   [Rpc(SendTo.Server)]
   public void SpawnItemsRpc()
   {
      
      
      itemSpawnPoints = GetItemSpawnPoints();
      for(int i = 0; i < itemSpawnPoints.Count; i++)
      {
          int type = itemSpawnPoints[i].GetComponent<itemSpawnType>().spawnType;
          StartCoroutine(SpawnItems(type, i));
        
      }
   }
  
   
   
   
   IEnumerator SpawnItems(int type, int spawnPointIndex)
   {
      for (int i = 0; i < spawnPointIndex; i++)
      {
         yield return new WaitForEndOfFrame();
      }
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
         case 3:
            SortRings(spawnPointIndex);
            break;
         case 4:
            SortNeckaces(spawnPointIndex);
            break;
         case 5:
            SortPans(spawnPointIndex);
            break;
      }
   }
   
   private void SortRings(int i)
   {
      int rnd = Random.Range(0, rings.Count);
      var instance = Instantiate(rings[rnd],itemSpawnPoints[i].transform);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
   }
   private void SortNeckaces(int i)
   {
      int rnd = Random.Range(0, necklaces.Count);
      var instance = Instantiate(necklaces[rnd],itemSpawnPoints[i].transform);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
   }
   
   private void SortPans(int i)
   {
      int rnd = Random.Range(0, pans.Count);
      var instance = Instantiate(pans[rnd],itemSpawnPoints[i].transform);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
   }
   
   
   void SortItemsCodigo(int x)
   {
      var instance = Instantiate(codigo,itemSpawnPoints[x].transform);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
      SortItemsQuadros(x);
   }

   private void SortItemsQuadros(int i)
   {
      int rnd = Random.Range(0, quadros.Count);
      var instance = Instantiate(quadros[rnd],itemSpawnPoints[i].transform);
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
