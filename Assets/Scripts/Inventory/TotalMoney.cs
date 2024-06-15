using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TotalMoney : NetworkBehaviour
{
    public static TotalMoney instance { get; private set; }
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    } 
   public NetworkVariable<int> totalMoney = new NetworkVariable<int>(0);
   
   
   [Rpc(SendTo.Server,RequireOwnership = false)]
   public void AddMoneyRpc(int money)
   {
       totalMoney.Value += money;
       
   }

   
}
