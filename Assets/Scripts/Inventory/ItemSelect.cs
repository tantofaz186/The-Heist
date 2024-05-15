using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelect : MonoBehaviour
{
   public int currentItem;
   
   PlayerInputActions controle_player;
   private InputAction item1,item2,item3,item4;
   
   private void Awake()
   {
      controle_player = new PlayerInputActions();
   }
   
   void OnEnable(){
      item1=controle_player.Player.Item1;
      item1.Enable();
      item1.performed += SelectItem1;
      item2=controle_player.Player.Item2;
      item2.Enable();
      item2.performed += SelectItem2;
      item3=controle_player.Player.Item3;
      item3.Enable();
      item3.performed += SelectItem3;
      item4=controle_player.Player.Item4;
      item4.Enable();
      item4.performed += SelectItem4;
   }

   private void OnDisable()
   {
      item1.Disable();
      item2.Disable();
      item3.Disable();
      item4.Disable();
      
   }


   void SelectItem1(InputAction.CallbackContext context){
      currentItem = 0;
      InventoryHud.Instance.ChangeActiveItem(currentItem);
   }
   void SelectItem2  (InputAction.CallbackContext context){
      currentItem = 1;
      InventoryHud.Instance.ChangeActiveItem(currentItem);
   } 
   void SelectItem3  (InputAction.CallbackContext context){
      currentItem = 2;
      InventoryHud.Instance.ChangeActiveItem(currentItem);
   }
   void SelectItem4  (InputAction.CallbackContext context){
      currentItem = 3;
      InventoryHud.Instance.ChangeActiveItem(currentItem);
   }
}
