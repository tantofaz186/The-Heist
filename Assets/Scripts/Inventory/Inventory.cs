using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    public static int SLOTS = 4;
    public static int MaxWeight = 10000;
    public List<Item> items = new ();
    public bool[] inventorySlots = new bool[SLOTS];
   [SerializeField] int emptySlot;
    public int itemCount;
    public int bagWeight;
    public List<Item> relics = new ();
    public int totalMoney;
    
    
    
   public void AddItem(Item item)
    {
        if (CheckEmptySlot())
        {
            items[emptySlot] = item;
            InventoryHud.Instance.AddItem(item, emptySlot);
            itemCount++;
            Debug.Log("Item Adicionado"+ " "+emptySlot);
        }
        else
        {
            Debug.Log("Inventory Full");
        }
    }

    public void AddRelic(Item item)
    {
        if(bagWeight+item.itemWeight<=MaxWeight)
        {
            relics.Add(item);
            bagWeight += item.itemWeight;
            totalMoney+= item.itemValue;
            InventoryHud.Instance.AddRelic(item);
            
        }
        else
        {
            Debug.Log("Bag Full");
        }
    }

    public void RemoveRelic()
    {
        if (relics.Count > 0)
        {
            bagWeight -= relics[relics.Count - 1].itemWeight;
            totalMoney -= relics[relics.Count - 1].itemValue;
            InventoryHud.Instance.RemoveRelic(relics[relics.Count - 1]);
            relics.RemoveAt(relics.Count - 1);
        }
    }

    public void RemoveItem(int itemPos)
    {
       
        items[itemPos]  = null;
        inventorySlots[itemPos] = false;
        itemCount--;
        InventoryHud.Instance.RemoveItem(itemPos);
        
    }

   
    
        
    

    bool CheckEmptySlot()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            if (inventorySlots[i] == false)
            {
                emptySlot = i;
                inventorySlots[i]= true;
                return true;
                
            }
        }
        
        return false;
    }

    void ResetValues()
    {
        items = new List<Item>();
        
    }

 
}
