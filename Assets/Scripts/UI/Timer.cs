using System;
using UnityEngine;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{   
    public static Timer instance { get; private set; }
   [SerializeField] public NetworkVariable<float> remainingTime;

   private void Awake()
   {
         instance = this;
       InvokeRepeating(nameof(CointTime), 1, 1);
         
   }


   void CointTime()
   {
       if(!IsServer) return;
       if (remainingTime.Value > 0)
       {
           remainingTime.Value--;
       }

       if (remainingTime.Value <= 0)
       {
           StopGameRpc();
       }
   }
   
    [Rpc(SendTo.Everyone,RequireOwnership = false)]
    void StopGameRpc()
    {
        Loader.Load(Loader.Scene.CombatReport);
        remainingTime.Value = 0;
    }
   

     
}
