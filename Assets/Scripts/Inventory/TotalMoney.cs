using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TotalMoney : NetworkBehaviour
{
    public static TotalMoney instance;
    
   public NetworkVariable<int> totalMoney = new NetworkVariable<int>(0);
   public TMP_Text moneyText;
   
   
   public void AddMoney(int money)
   {
       totalMoney.Value += money;
         UpdateText();
   }

   void UpdateText()
   {
       moneyText.text = totalMoney.Value.ToString();
   }
}
