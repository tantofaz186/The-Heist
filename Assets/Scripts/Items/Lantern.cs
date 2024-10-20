using System;
using Unity.Netcode;
using UnityEngine;

public class Lantern : BaseItem
{   
    public bool isOn;
    public GameObject _light;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isOn = false;
        ActivateLightRpc(false);
    }

    public override void UseItem()
    {
       isOn=!isOn;
       ActivateLightRpc(isOn);
    }
    
   [Rpc(SendTo.Everyone,RequireOwnership = false)]
   void ActivateLightRpc(bool state)
    {
        _light.SetActive(state);
    }
}
