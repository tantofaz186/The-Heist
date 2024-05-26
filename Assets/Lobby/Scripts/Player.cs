using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // void Update()
    // {
    //     if(!IsOwner)
    //     {
    //         return;
    //     }
    //
    //     HandleMovement();
    //     HandleInteractions();
    // }
    //
    // private void HandleInteractions()
    // {
    //     Debug.Log("Moving" + NetworkManager.Singleton.LocalClientId);
    // }
    //
    // private void HandleMovement()
    // {
    //     Debug.Log("Moving" + NetworkManager.Singleton.LocalClientId);
    // }
}
