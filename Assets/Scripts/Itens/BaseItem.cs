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

    public virtual void OnPick(ulong playerId)
    {
        HideItemRpc();
        ShowItemRpc();
    }

    public virtual void OnDrop() { }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void ShowItemRpc()
    {
        switch (item.type)
        {
            case Item.ItemType.NightVision:
                PlayerActions.Instance.nightVision.SetActive(true);
                break;
            case Item.ItemType.Flare:
                PlayerActions.Instance.flare.SetActive(true);
                break;
            case Item.ItemType.Key:
                PlayerActions.Instance.key.SetActive(true);
                break;
            case Item.ItemType.Radio:
                PlayerActions.Instance.radio.SetActive(true);
                break;
            case Item.ItemType.Relic:

            default:
                break;
        }
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void HideItemRpc()
    {
        Debug.Log($"Tentei esconder");
        meshRenderer.enabled = false;
        switch (item.type)
        {
            case Item.ItemType.NightVision:
                PlayerActions.Instance.nightVision.SetActive(false);
                OnDrop();
                break;
            case Item.ItemType.Flare:
                PlayerActions.Instance.flare.SetActive(false);
                break;
            case Item.ItemType.Key:
                PlayerActions.Instance.key.SetActive(false);
                break;
            case Item.ItemType.Radio:
                PlayerActions.Instance.radio.SetActive(false);
                break;
            case Item.ItemType.Relic:

            default:
                break;
        }

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