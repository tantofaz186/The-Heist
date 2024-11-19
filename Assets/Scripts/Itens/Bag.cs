using UnityEngine;

public class Bag : BaseItem
{
    private Inventory playerInventory;
    [SerializeField] private int bagWeight;

    public override void UseItem()
    {
        if (playerInventory != null)
        {
            playerInventory.MaxWeight = Mathf.Max(playerInventory.MaxWeight, bagWeight);
        }
    }

    public override void OnPick(ulong playerId)
    {
        playerInventory = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<Inventory>();
    }
}