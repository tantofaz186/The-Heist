using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private TextMeshPro useText;
    [SerializeField] public Camera camera;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] private LayerMask useLayers;
    [SerializeField] private float textDistance = 0.01f;


    public void OnUse()
    {
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
        {
            if(hit.collider.TryGetComponent<Door>(out Door door))
            {   
                
               if(door.isOpen)
               {
                   door.Close();
               }
               else
               {
                   door.Open(transform.position);
               }
            }
        }
    }

    private void Update()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, maxDistance,
                useLayers) && hit.collider.TryGetComponent<Door>(out Door door))
        {
            if (door.isOpen)
            {
                useText.SetText("Close \"E\"");
            }
            else
            {
                useText.SetText("Open \"E\"");
            }
            useText.gameObject.SetActive(true);
            useText.transform.position= hit.point-(hit.point-camera.transform.position).normalized*textDistance;
            useText.transform.rotation = Quaternion.LookRotation(hit.point - camera.transform.position).normalized;
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
    }
}
