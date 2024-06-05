using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrisonDoor : NetworkBehaviour
{

    [Rpc(SendTo.Server)]
    public void OpenServerRpc()
    {
        OpenRpc();
    }
    [Rpc(SendTo.Server)]
    public void CloseServerRpc()
    {
        CloseRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    public void OpenRpc()
    {
        gameObject.transform.Rotate(Vector3.up, 90);
    }
    [Rpc(SendTo.Everyone)]
    public void CloseRpc()
    {
        gameObject.transform.Rotate(Vector3.up, -90);
    }
}
