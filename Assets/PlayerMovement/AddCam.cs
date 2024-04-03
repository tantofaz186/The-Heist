using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AddCam : NetworkBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject head;

    private void Start()
    {   if(IsOwner)
        
        Instantiate(cam, cam.transform.position, cam.transform.rotation, head.transform);
    }
}
