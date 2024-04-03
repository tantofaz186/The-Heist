using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RoomSpawn : MonoBehaviour
{
   [SerializeField]List<GameObject> hallways = new List<GameObject>();
   [SerializeField] Transform mapCenter;
   [SerializeField] List<GameObject> roomSpawnpoints = new List<GameObject>();

   [SerializeField] private List<RoomTypes> themeRooms = new();
   [SerializeField]private List<int> themeRoomsused = new();
   [SerializeField] private List<RoomTypes> normalRooms = new();
   [SerializeField]private List<int> normalRoomsused = new();
   [SerializeField] private GameObject securityRoom;
   

   private void Start()
   {
       SortHallay();
       roomSpawnpoints = GetRoomSpawnPoints();
       SortRooms();
       
       
   }

   void SortHallay()
   {
      int rnd = Random.Range(0, hallways.Count);
      Instantiate(hallways[rnd],mapCenter);
   }

   List<GameObject> GetRoomSpawnPoints()
   {
     return GameObject.FindGameObjectsWithTag("RoomSpawnPoints").ToList();
      
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
       Debug.Log("End");
   }

   void SpawnThemeRoom(int x)
   {
       int rnd = Random.Range(0, themeRooms.Count);
      
       for (int i = 0; i < themeRooms.Count; i++)
       {   
          Debug.Log(i);
          
           if (themeRoomsused[i]==null||rnd != themeRoomsused[i])
           {
               themeRoomsused.Add(rnd);
               int rnd2 = Random.Range(0, themeRooms[rnd].Rooms.Count);
               Instantiate(themeRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position, Quaternion.identity);
               Debug.Log("innstanciou" + themeRooms[rnd].Rooms[rnd2].name);
               if(themeRoomsused.Count>=themeRooms.Count)
                   themeRoomsused.Clear();
               return;
           }
           
       }
       SpawnThemeRoom(x);
   }

   void SpawnNormalRoom(int x)
   {
       int rnd = Random.Range(0, normalRooms.Count);
       for (int i = 0; i < normalRoomsused.Count; i++)
       {   
           if(normalRoomsused.Count>=normalRooms.Count)
               normalRoomsused.Clear();
           if (rnd == normalRoomsused[i])
           {
               SpawnNormalRoom(x);
           }
           else
           {    
               normalRoomsused.Add(rnd);
               int rnd2 = Random.Range(0, normalRooms[rnd].Rooms.Count);
               Instantiate(normalRooms[rnd].Rooms[rnd2], roomSpawnpoints[x].transform.position, Quaternion.identity);
               
               
           }
       }
   }

   void SpawnSecurity()
   {
       int rnd = Random.Range(0, roomSpawnpoints.Count);
       Instantiate(securityRoom, roomSpawnpoints[rnd].transform.position, Quaternion.identity);
       roomSpawnpoints.Remove(roomSpawnpoints[rnd]);
   }
   
   
}
[Serializable]
public class RoomTypes
{
    public List<GameObject> Rooms = new();
}


