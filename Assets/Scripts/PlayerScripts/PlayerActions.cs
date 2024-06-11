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
    [SerializeField] float maxDistance = 5f;
    [SerializeField] private LayerMask useLayers;
    [SerializeField] private float textDistance = 0.01f;
    public bool ready = false;

    [SerializeField] public Image keySprite;

    

    private void Start()
    {
        if(!IsOwner)
        {
            enabled = false;
        }
        else
        {
           PlayerActionsSingleton.Instance.PlayerInputActions.Player.Use.performed += OpenCloseDoor;
        }
    }

    private void OnDisable()
    {
        if(!IsOwner) return;
        PlayerActionsSingleton.Instance.PlayerInputActions.Player.Use.performed -= OpenCloseDoor;
    }

    private void OpenCloseDoor(InputAction.CallbackContext obj)
    {
        if(!IsOwner)return; 
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


    private void Update()
    {
        if (!ready)
        {
            return;
        }
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance,
                useLayers))
        {
            
            if (hit.collider.TryGetComponent(out PickupObject pick))
            {   
                
                useText.SetText("Pick \"E\"");
                
                useText.gameObject.SetActive(true);
            }
            if (hit.collider.TryGetComponent(out Door door))
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
            
            Debug.Log("SEE");
            
            
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
    }
}
