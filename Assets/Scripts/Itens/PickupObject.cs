using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
public class PickupObject : NetworkBehaviour, Interactable
{
    public Item item;

    private Rigidbody m_Rigidbody;
    private BoxCollider m_Collider;

    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();

    [SerializeField]
    private Outline outline;

    private MeshRenderer _renderer;

    public NetworkVariable<bool> alreadyCollected = new();

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        _renderer = GetComponent<MeshRenderer>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        StartCoroutine(setActions());
    }

    public IEnumerator setActions()
    {
        yield return new WaitUntil(() => PlayerActions.Instance != null);
        PlayerActions.Instance.PlayerInputActions.Player.Release.performed += DropItem;
        PlayerActions.Instance.PlayerInputActions.Player.DropRelic.performed += DropRelic;
    }

    public override void OnDestroy()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Release.performed -= DropItem;
        PlayerActions.Instance.PlayerInputActions.Player.DropRelic.performed -= DropRelic;
        base.OnDestroy();
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

    private void DropItem(InputAction.CallbackContext obj)
    {
        if (IsOwner && !item.isRelic && m_IsGrabbed.Value)
        {
            Debug.Log($"Called {++calledTimes} times");
            if (Inventory.Instance.items[ItemSelect.Instance.currentItemIndex] == item)
            {
                ReleaseServerRpc(ItemSelect.Instance.currentItemIndex);
            }
        }
    }

    private static int calledTimes = 0;

    public string getDisplayText()
    {
        return "Pick \"E\"";
    }

    public void Interact()
    {
        TryGrabServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryGrabServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (m_IsGrabbed.Value) return;
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        NetworkObject senderPlayerObject = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
        if (senderPlayerObject == null) return;
        NetworkObject.ChangeOwnership(senderClientId);
        if (!item.isRelic)
        {
            TryGrabItemOwnerRpc(senderClientId);
        }
        else
        {
            TryGrabRelicOwnerRpc(senderClientId);
        }
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
    private void TryGrabItemOwnerRpc(ulong senderClientId)
    {
        if (Inventory.Instance.hasEmptySlot())
        {
            Inventory.Instance.AddItem(item);
            ParentObjectRpc(senderClientId);
            AddItemToCombatReportRpc();
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void AddItemToCombatReportRpc()
    {
        CombatReport.Instance.data.itensColetados++;
    }

    [Rpc(SendTo.Owner)]
    private void TryGrabRelicOwnerRpc(ulong senderClientId)
    {
        if (Inventory.Instance.bagWeight + item.itemWeight <= Inventory.MaxWeight)
        {
            Inventory.Instance.AddRelic(item);

            ParentObjectRpc(senderClientId);
        }
    }

    [ServerRpc]
    private void ReleaseServerRpc(int index)
    {
        CombatReport.Instance.data.itensColetados--;
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

    [Rpc(SendTo.Owner)]
    public void ReleaseItemOwnerRpc(int index)
    {
        Inventory.Instance.RemoveItem(index);
    }

    [ServerRpc]
    void DropRelicServerRpc()
    {
        ReleaseRelicOwnerRpc();
        NetworkObject.RemoveOwnership();
        UnparentObjectRpc();
    }

    [Rpc(SendTo.Owner)]
    public void ReleaseRelicOwnerRpc()
    {
        Inventory.Instance.RemoveRelic();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CollectRpc()
    {
        if (item.isRelic && !alreadyCollected.Value)
        {
            alreadyCollected.Value = true;
            Debug.Log("Relic Dropped");
            CombatReport.Instance.data.reliquiasColetadas++;
            CombatReport.Instance.data.dinheiroRecebido += item.itemValue;
            enabled = false;
        }
    }
}