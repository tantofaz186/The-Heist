using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : MonoBehaviourSingletonPersistent<InventoryHud>
{
    public Image[] slots = new Image[4];
    public GameObject[] borders = new GameObject[4];
    
    public void AddItem(Item item, int itemPos)
    {
      slots[itemPos].sprite = item.itemSprite;
      
    }
    public void RemoveItem(int itemPos)
    {
        slots[itemPos].sprite = null;
    }

    public void ChangeActiveItem(int current)
    {
        foreach (var VARIABLE in borders)
        {
            VARIABLE.SetActive(false);
        }
        borders[current].SetActive(true);
    }

    
}
