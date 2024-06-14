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

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.Grab.performed += Grab;
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.Release.performed += Release;
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.DropRelic.performed += DropRelic;
        
        _renderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
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
            ReleaseServerRpc(ItemSelect.Instance.currentItemIndex);
        }
    }

    private void Grab(InputAction.CallbackContext obj)
    {
        if (canGrab) TryGrabServerRpc();
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody)
        {
            m_Rigidbody.isKinematic = !IsServer || m_IsGrabbed.Value;
        }

        if (m_Collider)
        {
            m_Collider.isTrigger = !IsServer || m_IsGrabbed.Value;
        }
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (parentNetworkObject != null && (IsOwner || IsServer))
        {
            transform.localPosition = Vector3.up * 2;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryGrabServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (m_IsGrabbed.Value) return;
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        NetworkObject senderPlayerObject = PlayerPickup.Players[senderClientId].NetworkObject;
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
    
    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void ParentObjectRpc(ulong senderClientId)
    {
        NetworkObject senderPlayerObject = PlayerPickup.Players[senderClientId].NetworkObject;

        transform.parent = senderPlayerObject.transform;
        Transform playerTransform = senderPlayerObject.GetComponent<PlayerActions>().drop;
        transform.localPosition = playerTransform.position;
        m_IsGrabbed.Value = true;
        _renderer.enabled = false;    
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
        UnparentObjectRpc();
        NetworkObject.RemoveOwnership();
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void UnparentObjectRpc()
    {
        transform.parent = null;
        m_IsGrabbed.Value = false;
        _renderer.enabled = true;
        m_Rigidbody.AddForce(transform.up * 2, ForceMode.Impulse);
    }
    
    [ServerRpc]
    void DropRelicServerRpc()
    {
        if (Inventory.Instance.relics.Count > 0)
        {
            ReleaseRelicOwnerRpc();
            NetworkObject.RemoveOwnership();
            UnparentObjectRpc();
        }
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