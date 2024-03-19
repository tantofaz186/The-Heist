using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSpawn : MonoBehaviour
{
   [SerializeField]List<GameObject> hallways = new List<GameObject>();
   [SerializeField] Transform mapCenter;
   [SerializeField] List<GameObject> roomSpawnpoints = new List<GameObject>();

   [SerializeField] private List<RoomTypes> roomTypes = new();

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
   {
       foreach (var n in roomSpawnpoints)
       {
           int rndType = Random.Range(0, roomTypes.Count);
           int rndRoom = Random.Range(0, roomTypes[rndType].Rooms.Count);
           Instantiate(roomTypes[rndType].Rooms[rndRoom],n.transform);
       }
   }
   
}
[Serializable]
public class RoomTypes
{
    public List<GameObject> Rooms = new();
}
