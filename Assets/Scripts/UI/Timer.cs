using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{   
    public static Timer instance { get; private set; }
   [SerializeField] public NetworkVariable<float> remainingTime;

   private void Awake()
   {
         instance = this;
   }

   

   void Update()
    {   if(!IsServer) return;
        if (remainingTime.Value > 0)
        {
            remainingTime.Value-= Time.deltaTime;
        }

        if (remainingTime.Value <= 0)
        {
            StopGameRpc();
        }
    }
    [Rpc(SendTo.Everyone)]
    void StopGameRpc()
    {
        Debug.Log("Game Over");
        remainingTime.Value = 0;
    }
   

     
}
