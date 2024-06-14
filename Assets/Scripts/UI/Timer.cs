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
         Invoke(nameof(ChangeTime), 1f);
   }


   void ChangeTime()
   {
       if(!IsServer) return;
       if (remainingTime.Value > 0)
       {
           remainingTime.Value-= Time.deltaTime;
       }

       if (remainingTime.Value <= 0)
       {
           StopGameRpc();
       }
   }
   
    [Rpc(SendTo.Everyone,RequireOwnership = false)]
    void StopGameRpc()
    {
        Loader.Load(Loader.Scene.GameOver);
        remainingTime.Value = 0;
    }
   

     
}
