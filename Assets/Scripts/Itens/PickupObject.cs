using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
public class PickupObject : NetworkBehaviour
{
    public Item item;

    private bool canGrab;
    private Rigidbody m_Rigidbody;
    private BoxCollider m_Collider;

    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();

    [SerializeField]
    private Outline outline;

    private MeshRenderer _renderer;

    public bool alreadyCollected;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();

        _renderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private PlayerInputActions playerInputActions;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Use.performed += Grab;
        playerInputActions.Player.Release.performed += Release;
        playerInputActions.Player.DropRelic.performed += DropRelic;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerInputActions.Player.Use.performed -= Grab;
        playerInputActions.Player.Release.performed -= Release;
        playerInputActions.Player.DropRelic.performed -= DropRelic;
        playerInputActions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("View"))
        {
            canGrab = true;
            outline.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("View"))
        {
            canGrab = false;
            outline.enabled = false;
        }
    }

    private void DropRelic(InputAction.CallbackContext obj)
    {
        if (m_IsGrabbed.Value && IsOwner)
        {
            if (item.isRelic)
            {
                DropRelicServerRpc();
            }
        }
    }

    private void Release(InputAction.CallbackContext obj)
    {
        if (IsOwner && !item.isRelic && m_IsGrabbed.Value)
        {
            Debug.Log($"Called {++calledTimes} times");
            if (Inventory.Instance.items[ItemSelect.Instance.currentItemIndex] == item)
                ReleaseServerRpc(ItemSelect.Instance.currentItemIndex);
        }
    }

    private static int calledTimes = 0;

    private void Grab(InputAction.CallbackContext obj)
    {
        if (canGrab) TryGrabServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryGrabServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (m_IsGrabbed.Value) return;
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        NetworkObject senderPlayerObject = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
        if (senderPlayerObject == null) return;
        NetworkObject.ChangeOwnership(senderClientId);
        if (!item.isRelic && Inventory.Instance.hasEmptySlot())
        {
            TryGrabItemOwnerRpc();
        }
        else if (Inventory.Instance.bagWeight + item.itemWeight <= Inventory.MaxWeight)
        {
            TryGrabRelicOwnerRpc();
        }
        else
        {
            return;
        }

        ParentObjectRpc(senderClientId);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void ParentObjectRpc(ulong senderClientId)
    {
        NetworkObject senderPlayerObject = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
        Transform playerTransform = senderPlayerObject.GetComponent<PlayerActions>().drop;
        transform.parent = senderPlayerObject.transform;
        transform.localPosition = playerTransform.localPosition;
        m_IsGrabbed.Value = true;
        m_Collider.enabled = false;
        SomeRpc(false);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void SomeRpc(bool _enabled)
    {
        _renderer.enabled = _enabled;
        m_Rigidbody.isKinematic = !_enabled;
        m_Collider.isTrigger = !_enabled;
    }

    [Rpc(SendTo.Owner)]
    private void TryGrabItemOwnerRpc()
    {
        Inventory.Instance.AddItem(item);
    }

    [Rpc(SendTo.Owner)]
    private void TryGrabRelicOwnerRpc()
    {
        Inventory.Instance.AddRelic(item);
    }

    [ServerRpc]
    private void ReleaseServerRpc(int index)
    {
        ReleaseItemOwnerRpc(index);
        NetworkObject.RemoveOwnership();
        UnparentObjectRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void UnparentObjectRpc()
    {
        transform.parent = null;
        m_IsGrabbed.Value = false;
        _renderer.enabled = true;
        m_Collider.enabled = true;
        SomeRpc(true);
        m_Rigidbody.AddForce(transform.forward * 0.5f, ForceMode.Impulse);
    }

    [ServerRpc]
    void DropRelicServerRpc()
    {
        ReleaseRelicOwnerRpc();
        NetworkObject.RemoveOwnership();
        UnparentObjectRpc();
    }

    [Rpc(SendTo.Owner)]
    public void ReleaseItemOwnerRpc(int index)
    {
        Inventory.Instance.RemoveItem(index);
    }

    [Rpc(SendTo.Owner)]
    public void ReleaseRelicOwnerRpc()
    {
        Inventory.Instance.RemoveRelic();
    }
}