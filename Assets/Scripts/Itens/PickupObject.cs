using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PickupObject : NetworkBehaviour
{
    public Item item;
    public float GrabDistance = 2.0f;

    private Rigidbody m_Rigidbody;
    private BoxCollider m_Collider;

    private NetworkVariable<bool> m_IsGrabbed = new NetworkVariable<bool>();
    private PlayerInputActions controle_player;
    private InputAction grab, release;
    
    private MeshRenderer _renderer;
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        controle_player = new PlayerInputActions();
        grab = controle_player.Player.Grab;
        grab.Enable();
        grab.performed += Grab;

        release = controle_player.Player.Release;
        release.Enable();
        release.performed += Release;
        _renderer = GetComponent<MeshRenderer>();
    }

    private void Release(InputAction.CallbackContext obj)
    {
        if (m_IsGrabbed.Value && IsOwner)
        {
            ReleaseServerRpc();
        }
    }

    private void Grab(InputAction.CallbackContext obj)
    {
        if (m_IsGrabbed.Value) return;
        var localPlayerObject = NetworkManager?.SpawnManager?.GetLocalPlayerObject();
        if (localPlayerObject == null) return;
        var distance = Vector3.Distance(transform.position, localPlayerObject.transform.position);
        var cos = Vector3.Dot(transform.forward, (localPlayerObject.transform.position - transform.position).normalized);
        if (!(distance <= GrabDistance) && cos <= 0.5f) return;
        
        TryGrabServerRpc();
        
    }

    private void FixedUpdate()
    {
        if (NetworkManager == null)
        {
            return;
        }

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
        if (!m_IsGrabbed.Value)
        {
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var senderPlayerObject = PlayerPickup.Players[senderClientId].NetworkObject;
            if (senderPlayerObject != null)
            {
                NetworkObject.ChangeOwnership(senderClientId);

                //send item to inventory
                if(Inventory.Instance.itemCount< Inventory.SLOTS)
                {
                    Inventory.Instance.AddItem(item);
                    transform.parent = senderPlayerObject.transform;
                    transform.localPosition = new Vector3(0.473f, 0.605f, -0.314f);
                    m_IsGrabbed.Value = true;
                    _renderer.enabled = false;
                }
            }
        }
    }

    [ServerRpc]
    private void ReleaseServerRpc()
    {
        
        
        if(Inventory.Instance.inventorySlots[ItemSelect.Instance.currentItem]==true)
        {   
           
            if (m_IsGrabbed.Value)
                    { 
                        Inventory.Instance.RemoveItem(ItemSelect.Instance.currentItem);
                        NetworkObject.RemoveOwnership();
                        transform.parent = null;
                        m_IsGrabbed.Value = false;
                        _renderer.enabled = true;
                    }
        }
    }
}
