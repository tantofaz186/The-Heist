using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviourSingletonPersistent<Inventory>
{
    public static int SLOTS = 4;
    public List<Item> items = new ();
    public bool[] inventorySlots = new bool[SLOTS];
   [SerializeField] int emptySlot;
    public int itemCount;
    
    
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

    public void RemoveItem(int itemPos)
    {
        Instantiate(items[itemPos].itemPrefab, transform.position, Quaternion.identity);
        items.RemoveAt(itemPos);
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

 
}
