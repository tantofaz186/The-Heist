using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPick : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
   {
      if(other.CompareTag("Item"))
      { 
         if(Inventory.instance.itemCount< Inventory.SLOTS)
         {
            Item item = other.GetComponent<ItemBehaviour>().item;
                     Inventory.instance.AddItem(item);
                     Destroy(other.gameObject);
         }
      }
   }
}
