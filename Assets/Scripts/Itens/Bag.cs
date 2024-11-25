using UnityEngine;

public class Bag : BaseItem
{
    private Inventory playerInventory;
    [SerializeField] private int bagWeight;

    public override void UseItem()
    {

    }

    public override void OnPick(ulong playerId)
    {
        base.OnPick(playerId);
        playerInventory = NetworkManager.SpawnManager.GetPlayerNetworkObject(playerId).GetComponent<Inventory>();
    }
}