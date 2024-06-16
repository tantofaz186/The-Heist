using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelect : Singleton<ItemSelect>
{  
   public BaseItem itemInHand;

   public int currentItemIndex;
   private PlayerInputActions playerInputActions;

   private void Start()
   {
      playerInputActions = new PlayerInputActions();
      playerInputActions.Enable();
      playerInputActions.Player.Enable();
      playerInputActions.Player.Item1.performed += SelectItem1;
      playerInputActions.Player.Item2.performed += SelectItem2;
      playerInputActions.Player.Item3.performed += SelectItem3;
      playerInputActions.Player.Item4.performed += SelectItem4;
   }

   void SelectItem1(InputAction.CallbackContext context){
      currentItemIndex = 0;
      UpdateBaseItem();
   }
   void SelectItem2  (InputAction.CallbackContext context){
      currentItemIndex = 1;
      UpdateBaseItem();
   } 
   void SelectItem3  (InputAction.CallbackContext context){
      currentItemIndex = 2;
      UpdateBaseItem();
   }
   void SelectItem4  (InputAction.CallbackContext context){
      currentItemIndex = 3;
      UpdateBaseItem();
   }


   void UpdateBaseItem()
   {
      InventoryHud.Instance.ChangeActiveItem(currentItemIndex);
      if (Inventory.Instance.items[currentItemIndex] != null)
      {
         Inventory.Instance.itemsInHand[Inventory.Instance.items[currentItemIndex].prefabIndex].SetActive(true);
         itemInHand = Inventory.Instance.itemsInHand[Inventory.Instance.items[currentItemIndex].prefabIndex].GetComponent<BaseItem>();
      }
      else
      {
         itemInHand = null;
      }
   }
}
