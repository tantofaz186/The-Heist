using Unity.Netcode;
using UnityEngine;

public class Lantern : BaseItem
{   public bool isOn = false;
    public GameObject _light;
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
