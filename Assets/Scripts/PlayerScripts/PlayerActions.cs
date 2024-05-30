using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerActions : NetworkBehaviour
{
    [SerializeField] public TMP_Text useText;
    [SerializeField] public Camera camera;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] private LayerMask useLayers;
    [SerializeField] private float textDistance = 0.01f;
    public bool ready = false;


    public void OnUse()
    {
        if(Physics.Raycast(this.transform.position, camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
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
    }

    private void Update()
    {
        if (!ready)
        {
            return;
        }
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance,
                useLayers) && hit.collider.TryGetComponent<Door>(out Door door))
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
        else
        {
            useText.gameObject.SetActive(false);
        }
    }
}
