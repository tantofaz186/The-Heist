using UnityEngine;

public class Bag : BaseItem
{
    private Inventory playerInventory;
    [SerializeField] private int bagWeight;

    public override void UseItem()
    {
        Debug.Log(playerInventory != null ? "Player Inventory Found" : "Player Inventory Not Found");
        if (playerInventory == null) return;
        if(playerInventory.MaxWeight >= bagWeight) return;
        playerInventory.MaxWeight = Mathf.Max(playerInventory.MaxWeight, bagWeight);
        HideItemRpc();
        playerInventory.RemoveItem(item);
    }

    public override void OnPick(ulong playerId)
    {
        playerInventory = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<Inventory>();
    }
}