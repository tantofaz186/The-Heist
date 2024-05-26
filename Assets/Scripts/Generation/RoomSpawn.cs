using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class RoomSpawn : NetworkBehaviour
{
   [SerializeField]List<GameObject> hallways = new List<GameObject>();
   [SerializeField] Transform mapCenter;
   [SerializeField] List<GameObject> roomSpawnpoints = new List<GameObject>();

   [SerializeField] private List<RoomTypes> themeRooms = new();
   [SerializeField]private List<int> themeRoomsCheck = new();
   [SerializeField] private List<RoomTypes> normalRooms = new();
   [SerializeField]private List<int> normalRoomsCheck = new();
   [SerializeField] private GameObject securityRoom;
   [SerializeField] public List<GameObject> playerSpawnPoints = new();
   

   public NavMeshBake bake;
   public EnemySpawn enemySpawn;
   public ItemSpawn itemSpawn;
  
   public override void OnNetworkSpawn()
   {
       base.OnNetworkSpawn();
       if (IsServer)
       {
           SpawnSceneRpc();
       }

       StartCoroutine(WaitToBake());
   }

   private IEnumerator WaitToBake()
   {
       yield return new WaitForSeconds(1);
       yield return new WaitUntil(() => bake.surface.navMeshData != null || bake.surface != null);
       bake.Bake();
       yield return new WaitForSeconds(1);
       if (IsServer)
       {
           SpawnEnemyRpc();
       }
   }

   [Rpc(SendTo.Everyone)]
   void SpawnSceneRpc()
   {
       SortHallay();
       roomSpawnpoints = GetRoomSpawnPoints();
       SortRooms();
   }
   
   [Rpc(SendTo.Everyone)]
   void SpawnEnemyRpc()
   {
       enemySpawn.SpawnEnemyRpc();
       itemSpawn.SpawnItemsRpc();
   }

   
  

   void SortHallay()
   {
      int rnd = Random.Range(0, hallways.Count);
      var instance = Instantiate(hallways[rnd],mapCenter);
      var instanceNetworkObject = instance.GetComponent<NetworkObject>();
      instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
   }

   List<GameObject> GetRoomSpawnPoints()
   {
     return GameObject.FindGameObjectsWithTag("RoomSpawnPoints").ToList();
      
   }
  
   
   List<GameObject> GetSpawnPoints()
   {
       return GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
      
   }

   void SortRooms()
   {    SpawnSecurity();
       for (int n = 0; n < roomSpawnpoints.Count; n++)
       {    
           int type = roomSpawnpoints[n].GetComponent<SpawnInfo>().spawnType;
           switch (type)
           {
               case 0:
                   SpawnThemeRoom(n);
                   
                   break;
               case 1:
                   SpawnNormalRoom(n);
                   
                  break;
           }
       }
      
   }

   void SpawnThemeRoom(int x)
   {
       int rnd = Random.Range(0, themeRooms.Count);
      
       bool inList = false;
       if (themeRoomsCheck.Count <= 0)
       {
           themeRoomsCheck = new List<int>(){0,1,2,3};
          
       }
           
       
       for (int i=0;i<themeRoomsCheck.Count;i++)
       {   
           if (rnd == themeRoomsCheck[i])
           {   
               inList = true;
           }  
       }

       if (inList)
       {    
           themeRoomsCheck.Remove(rnd);
           
           int rnd2 = Random.Range(0, themeRooms[rnd].Rooms.Count);
          var instance =  Instantiate(themeRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position,roomSpawnpoints[x].transform.rotation);
           var instanceNetworkObject = instance.GetComponent<NetworkObject>();
           instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
       }
       else
       {
           SpawnThemeRoom(x);
       }
   }

   void SpawnNormalRoom(int x)
   {
       int rnd = Random.Range(0, normalRooms.Count);
      
       bool inList = false;
       if (normalRoomsCheck.Count <= 0)
       {
           normalRoomsCheck = new List<int>(){0,1,2,3};
          
       }
           
       
       for (int i=0;i<normalRoomsCheck.Count;i++)
       {   
           if (rnd == normalRoomsCheck[i])
           {   
               inList = true;
           }  
       }

       if (inList)
       {    
           normalRoomsCheck.Remove(rnd);
           
           int rnd2 = Random.Range(0, normalRooms[rnd].Rooms.Count);
          var instance =  Instantiate(normalRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position,roomSpawnpoints[x].transform.rotation);
           var instanceNetworkObject = instance.GetComponent<NetworkObject>();
           instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
       }
       else
       {
           SpawnNormalRoom(x);
       }
   }

   void SpawnSecurity()
   {
       int rnd = Random.Range(0, roomSpawnpoints.Count);
       var instance = Instantiate(securityRoom, roomSpawnpoints[rnd].transform.position, roomSpawnpoints[rnd].transform.rotation);
       var instanceNetworkObject = instance.GetComponent<NetworkObject>();
       instanceNetworkObject.SpawnWithOwnership(OwnerClientId);

       roomSpawnpoints.Remove(roomSpawnpoints[rnd]);
   }
   
   
}
[Serializable]
public class RoomTypes
{
    public List<GameObject> Rooms = new();
}


