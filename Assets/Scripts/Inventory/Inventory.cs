using System;
using System.Collections.Generic;
using CombatReportScripts;
using Unity.Netcode;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    public static int SLOTS = 4;
    public static int MaxWeight = 10000;
    public Item[] items = new Item[SLOTS];
    public int bagWeight;
    public List<Item> relics = new();
    public int totalMoney;

    public GameObject[] itemsInHand;

    public static Inventory Instance;

    CombatReportBehaviour playerCombatReport;
    public void AddItem(Item item)
    {
        int emptySlotIndex = CheckEmptySlot();
        if (emptySlotIndex > -1)
        {
            items[emptySlotIndex] = item;
            InventoryHud.Instance.AddItem(item, emptySlotIndex);
            Debug.Log("Item Adicionado" + " " + emptySlotIndex);
            playerCombatReport.combatReportData.itensColetados++;
        }
        else
        {
            Debug.Log("Inventory Full");
        }
    }

    private void Start()
    {
        playerCombatReport = GetComponent<CombatReportBehaviour>();
    }

    public void AddRelic(Item item)
    {
        if (bagWeight + item.itemWeight <= MaxWeight)
        {
            relics.Add(item);
            bagWeight += item.itemWeight;
            totalMoney += item.itemValue;
            InventoryHud.Instance.AddRelic(item);
            playerCombatReport.combatReportData.reliquiasColetadas++;
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
            bagWeight -= relics[^1].itemWeight;
            totalMoney -= relics[^1].itemValue;
            InventoryHud.Instance.RemoveRelic(relics[^1]);
            relics[^1] = null;
            relics.RemoveAll((i) => i == null);
        }
    }

    public bool RemoveItem(int itemPos)
    {
        if (items[itemPos] == null)
        {
            return false;
        }

        Debug.Log($"Item Removido {itemPos}");
        items[itemPos] = null;
        InventoryHud.Instance.RemoveItem(itemPos);
        return true;
    }

    int CheckEmptySlot()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public bool hasEmptySlot()
    {
        return CheckEmptySlot() > -1;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        Instance = this;
    }
}