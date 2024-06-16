using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLocation : MonoBehaviour
{
  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Item"))
    {
        other.TryGetComponent(out PickupObject item);
        if (item.item.isRelic&& item.alreadyCollected==false)
        {Debug.Log("Relic Dropped");
            TotalMoney.instance.AddMoneyRpc(item.item.itemValue);
            TotalMoney.instance.AddItemsCountRpc(1);
            item.enabled=false;
            item.alreadyCollected=true;
            
        }
    }
  }

  
  
}


