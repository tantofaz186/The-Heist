using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerActions : NetworkBehaviour
{
    [SerializeField] public TMP_Text useText;
    [SerializeField] public Camera camera;
    [SerializeField] private LayerMask useLayers;
    [SerializeField] private float maxDistance = 5f;
    public bool ready = false;
    [SerializeField] public Image keySprite;

    private PlayerInputActions playerInputActions;


    
    public void OnUse()
    {   if(!IsOwner)return;
        Debug.Log("E");
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
        {
            if(hit.collider.TryGetComponent<Door>(out Door door))
            {   
                
               if(door.isOpen.Value)
               {
                   door.CloseServerRpc();
               }
               else
               {
                   door.OpenServerRpc(transform.position);
               }
            }
        }
    }

    private void Start()
    {
        if(!IsOwner)
        {
            this.enabled= false;
        }
        else
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
playerInputActions.Player.Use.performed += OnUse;
        }
    }

    private void OnUse(InputAction.CallbackContext obj)
    {
        OnUse();
    }

    private void Update()
    {   if(!IsOwner)return;
        if (!ready)
        {
            return;
        }
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
        {
            if (hit.collider.TryGetComponent<PickupObject>(out PickupObject item))
            {
                useText.SetText("Pick \"E\"");
                
                useText.gameObject.SetActive(true);
            }
             else if (hit.collider.TryGetComponent<Door>(out Door door))
            {
                if (door.isOpen.Value)
                {
                    useText.SetText("Close \"E\"");
                }
                else
                {
                    useText.SetText("Open \"E\"");
                }

                useText.gameObject.SetActive(true);
                
            }
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
           
        
    }
}
