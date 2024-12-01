using UnityEngine;

public class Bag : BaseItem
{
    private Inventory playerInventory;

    [SerializeField]
    private int bagWeight;

    public override void UseItem() { }

    public override void OnPick(ulong playerId)
    {
        base.OnPick(playerId);
        playerInventory = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<Inventory>();
        Debug.Log(playerInventory != null ? "Player Inventory Found" : "Player Inventory Not Found");
        if (playerInventory == null) return;
        if (playerInventory.MaxWeight >= bagWeight)
        {
            GetComponent<PickupObject>().ForceDropItem();
            InventoryHud.Instance.TryRefresh();
            return;
        }

        playerInventory.MaxWeight = Mathf.Max(playerInventory.MaxWeight, bagWeight);
        HideItemRpc();
        playerInventory.RemoveItem(item);
    }
}