using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public abstract class BaseItem : NetworkBehaviour
{
    [SerializeField]
    protected Item item;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    protected GameObject visualEffect;

    private NetworkObject spawnedObjectVfx;
    public abstract void UseItem();
    public virtual void OnPick(ulong playerId){}
    public virtual void OnDrop(){}

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void ShowItemRpc()
    {
        meshRenderer.enabled = true;
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void HideItemRpc()
    {
        Debug.Log($"Tentei esconder");
        meshRenderer.enabled = false;
        if (IsServer)
        {
            if (spawnedObjectVfx.IsSpawned)
            {
                Debug.Log($"Entrei aqui");
                spawnedObjectVfx.Despawn();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"Spawned item : {item.itemName} as object: {gameObject.name}");
        if (IsServer)
        {
            var instance = Instantiate(visualEffect, transform.position, Quaternion.identity);
            spawnedObjectVfx = instance.GetComponent<NetworkObject>();
            spawnedObjectVfx.Spawn();
            spawnedObjectVfx.TrySetParent(transform);
            spawnedObjectVfx.transform.localPosition = Vector3.zero;
        }
    }

    protected virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}