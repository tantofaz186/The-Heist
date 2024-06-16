using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheHeistGameManager : NetworkBehaviour
{
  [SerializeField] List<GameObject> playerspawns= new List<GameObject>();

  private void Start()
  {
     // NetworkManager.Singleton.ConnectionApprovalCallback+= ApprovalCheck;
  }
  
  /*void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,NetworkManager.ConnectionApprovalResponse callback)
  {
      bool approve = true;
      bool createPlayerObject = true;
      ulong? prefabHash = null;
      
    
      
  }*/

  List<GameObject> GetSpawnPoints()
   {
       return GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
   }

    [SerializeField] private PlayerPrefab playerPrefab = new();
    private int count;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            Debug.Log("Clients connected: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        }
    }
    
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        playerspawns = GetSpawnPoints();
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            int colorIndex = TheHeistGameMultiplayer.Instance.GetPlayerDataFromClient(clientId).colorId;
            GameObject playerTransform = Instantiate(playerPrefab.prefabColors[colorIndex], playerspawns[count].transform.position, playerspawns[count].transform.rotation);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            count++;
           
           // Color colorPlayer = TheHeistGameMultiplayer.Instance.GetPlayerColor(colorIndex);
           // SkinnedMeshRenderer playerMesh = playerTransform.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>();
           // playerMesh.material.color = colorPlayer;
            Debug.Log("Spawned " + playerTransform.name + " for client " + clientId);
        }
    }

   
    
}

[Serializable]
public class PlayerPrefab
{
    public List<GameObject> prefabColors = new();
}
