using System;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseItem : NetworkBehaviour
{
    [SerializeField] protected Item item;
    [SerializeField] private MeshRenderer meshRenderer;
    public abstract void UseItem();

    [Rpc(SendTo.Everyone,RequireOwnership = false)]
    public void ShowItemRpc()
    {
        meshRenderer.enabled = true;
    }
    
    [Rpc(SendTo.Everyone,RequireOwnership = false)]
    public void HideItemRpc()
    {
        meshRenderer.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"Spawned item : {item.itemName} as object: {gameObject.name}");
    }

    protected virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
