using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{   public static Inventory instance { get; private set; }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static int SLOTS = 4;
    public List<Item> items = new ();
    public bool[] inventorySlots = new bool[SLOTS];
    int emptySlot;
    public int itemCount;
    
    
   public void AddItem(Item item)
    {
        if (CheckEmptySlot())
        {
            items[emptySlot] = item;
            InventoryHud.instance.AddItem(item, emptySlot);
            itemCount++;
        }
        else
        {
            Debug.Log("Inventory Full");
        }
        
        
    }

    public void RemoveItem(int itemPos)
    {
        //drop item
        items.RemoveAt(itemPos);
        inventorySlots[itemPos] = false;
        itemCount--;
        InventoryHud.instance.RemoveItem(itemPos);
        
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

 
}
