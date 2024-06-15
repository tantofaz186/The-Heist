using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : Singleton<InventoryHud>
{
    public Image[] slots = new Image[4];
    public GameObject[] borders = new GameObject[4];
    public Slider weighSlider;
    public Text moneyText;
    public void AddItem(Item item, int itemPos)
    {
      slots[itemPos].sprite = item.itemSprite;
      slots[itemPos].color = new Color(slots[itemPos].color.r, slots[itemPos].color.g, slots[itemPos].color.b, 1f);

    }
    public void RemoveItem(int itemPos)
    {
        Debug.Log($"removed item at pos: {itemPos}");
        slots[itemPos].color = new Color(slots[itemPos].color.r, slots[itemPos].color.g, slots[itemPos].color.b, 0f);
        slots[itemPos].sprite = null;
    }

    public void AddRelic(Item item)
    {
        weighSlider.value+= item.itemWeight;
    }
    
    public void RemoveRelic(Item item)
    {
        weighSlider.value-= item.itemWeight;
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
