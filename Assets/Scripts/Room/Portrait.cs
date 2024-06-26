using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class Portrait :NetworkBehaviour, Interactable
{
  public NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
  [SerializeField] Animator portraitAnimator;
  [Rpc(SendTo.Server,RequireOwnership = false)]
  public void OpenServerRpc()
  {
    OpenRpc();
    isOpen.Value=true;
  }
  
  
  
  [Rpc(SendTo.Everyone,RequireOwnership = false)]
  public  void OpenRpc()
  { 
    if (!isOpen.Value)
    {
      portraitAnimator.SetTrigger("interact");
    }
  }
  
  
  [Rpc(SendTo.Server)]
  private void ServerOpenRpc()
  {
    isOpen.Value=true;
  }
  
  public string getDisplayText()
  {
    return isOpen.Value?"": "Interact \"E\"";
  }

  public void Interact()
  {
    if (!isOpen.Value)
    {
      OpenServerRpc();
    }
  }
}
