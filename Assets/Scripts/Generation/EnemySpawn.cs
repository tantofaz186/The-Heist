using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawn : NetworkBehaviour
{
     [SerializeField] public List<GameObject> enemySpawnPoints = new();
   
     [SerializeField] public List<GameObject> enemyList = new();
     
   [Rpc(SendTo.Server)]
   public void SpawnEnemyRpc()
    {   
        enemySpawnPoints = GetRoomSpawnEnemy();
        for(int i=0;i<enemySpawnPoints.Count;i++)
        {
          
           var instance = Instantiate(enemyList[i], enemySpawnPoints[i].transform.position, enemySpawnPoints[i].transform.rotation);
           var instanceNetworkObject = instance.GetComponent<NetworkObject>();
           instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
        }
       
    }
       List<GameObject> GetRoomSpawnEnemy()
    {
        return GameObject.FindGameObjectsWithTag("EnemySpawnPoints").ToList();
      
    }
}
