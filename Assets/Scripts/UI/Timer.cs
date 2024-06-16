using System;
using UnityEngine;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{   
    public static Timer instance { get; private set; }
   [SerializeField] public NetworkVariable<float> remainingTime;
   [SerializeField] public NetworkVariable<float> totalTime;
   [SerializeField] private float startTime;
   private void Awake()
   {
         instance = this;
         totalTime.Value = startTime;
         remainingTime.Value = startTime;
         
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
    {      Debug.Log("StopGameRpc");
        CombatReportData.instance.AtualzarDados();
        Loader.Load(Loader.Scene.CombatReportScene);
        remainingTime.Value = 0;
    }
   

     
}
