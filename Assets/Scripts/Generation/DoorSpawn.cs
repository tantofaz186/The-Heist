using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DoorSpawn : NetworkBehaviour
{
    [SerializeField] List<GameObject> doorSpawnPoints = new();
    [SerializeField] List<GameObject> doorPrefabs = new();
    
    List<GameObject> GetDoorSpawnPoints()
    {
        return GameObject.FindGameObjectsWithTag("DoorSpawnPoints").ToList();
      
    }
    [Rpc(SendTo.Server)]
    public void SpawnDoorsRpc()
    {
        doorSpawnPoints = GetDoorSpawnPoints();
        for (int i = 0; i < doorSpawnPoints.Count; i++)
        {
            int rnd = Random.Range(0, doorPrefabs.Count);
            var instance = Instantiate(doorPrefabs[rnd], doorSpawnPoints[i].transform.position,doorPrefabs[rnd].transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
        }
       
    }
}
