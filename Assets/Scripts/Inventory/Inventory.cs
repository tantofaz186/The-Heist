using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatReportScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : NetworkBehaviour
{
    public const int SLOTS = 4;
    public int MaxWeight = 100;
    public Item[] items = new Item[SLOTS];

    [FormerlySerializedAs("bagWeight")]
    public int currentWeight;

    public List<Item> relics = new();
    public int totalMoney;

    public GameObject[] itemsInHand = new GameObject[SLOTS];

    public static Inventory Instance;

    CombatReportBehaviour playerCombatReport;

    public void AddItem(GameObject go, Item item)
    {
        int emptySlotIndex = CheckEmptySlot();
        if (emptySlotIndex > -1)
        {
            items[emptySlotIndex] = item;
            itemsInHand[emptySlotIndex] = go;
            InventoryHud.Instance.AddItem(item, emptySlotIndex);
            Debug.Log("Item Adicionado" + " " + emptySlotIndex + " : " + item.itemName);
            playerCombatReport.combatReportData.itensColetados++;
        }
        else
        {
            Debug.Log("Inventory Full");
            StartCoroutine(DisplayFullInventoryMessage());
        }
    }

    private IEnumerator DisplayFullInventoryMessage()
    {
        InventoryHud.Instance.DisplayFullInventoryMessage(true);
        yield return new WaitForSeconds(2);
        InventoryHud.Instance.DisplayFullInventoryMessage(false);
    }

    private void Start()
    {
        playerCombatReport = GetComponent<CombatReportBehaviour>();
        items = new Item[SLOTS];
        itemsInHand = new GameObject[SLOTS];
    }

    public bool AddRelic(Item item)
    {
        if (currentWeight + item.itemWeight <= MaxWeight)
        {
            relics.Add(item);
            currentWeight += item.itemWeight;
            totalMoney += item.itemValue;
            InventoryHud.Instance.ChangeWeight();
            playerCombatReport.combatReportData.reliquiasColetadas++;
            return true;
        }

        Debug.Log("Bag Full");
        return false;
    }

    public void RemoveRelic()
    {
        if (relics.Count > 0)
        {
            currentWeight -= relics[^1].itemWeight;
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
        itemsInHand[itemPos] = null;
        ItemSelect.Instance.ConsumeItem();
        InventoryHud.Instance.RemoveItem(itemPos);
        return true;
    }

    public bool RemoveItem(Item _item)
    {
        int index = items.ToList().FindIndex((i) => i == _item);
        if (index == -1)
        {
            return false;
        }

        return RemoveItem(index);
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