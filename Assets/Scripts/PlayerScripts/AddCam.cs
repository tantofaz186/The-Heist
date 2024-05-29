using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AddCam : NetworkBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject head;
    private PlayerActions _actions;
    private void Start()
    {
        if (IsOwner)
        {
            var thisCam = Instantiate(cam, head.transform.position, cam.transform.rotation, head.transform);
             _actions = GetComponent<PlayerActions>();
             _actions.camera = thisCam.GetComponent<Camera>();
        }
       
    
    }
}
