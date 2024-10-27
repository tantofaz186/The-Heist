using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Map : NetworkBehaviour,Interactable
{
     
    public string getDisplayText()
    {
        return "Pick Map";
    }

    public void Interact()
    {
       PickMapRpc();
    }
    [Rpc(SendTo.Everyone)]
    void PickMapEveryoneRpc()
    {
        var playerStats = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStats>();
        playerStats.mapPieces++;
       if(playerStats.mapPieces >=3)
        {
            GameObject.FindGameObjectWithTag("Minimap").GetComponent<Image>().enabled = true;
        }
       this.gameObject.SetActive(false);
    }
    
    [Rpc(SendTo.Server)]
   void PickMapRpc()
    {
            PickMapEveryoneRpc();
    }
}
