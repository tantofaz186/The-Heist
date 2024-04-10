using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemySpawnPoints = new();
   
    [SerializeField] public List<GameObject> enemyList = new();
    
   public void SpawnEnemy()
    {
        enemySpawnPoints = GetRoomSpawnEnemy();
        for(int i=0;i<enemySpawnPoints.Count;i++)
        {
            int rnd = Random.Range(0, enemyList.Count);
            Instantiate(enemyList[rnd], enemySpawnPoints[i].transform.position, enemySpawnPoints[i].transform.rotation);
        }
       
    }
   
    List<GameObject> GetRoomSpawnEnemy()
    {
        return GameObject.FindGameObjectsWithTag("EnemySpawnPoints").ToList();
      
    }
}
