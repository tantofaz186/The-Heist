using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class AddCam : NetworkBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] GameObject head;
    private PlayerActions _actions;
    public TMP_Text useText;
    [SerializeField] private GameObject canvas;
    [SerializeField]GameObject useTextobj;
    private void Start()
    {
        if (IsOwner)
        {   
            var thisCam = Instantiate(cam, head.transform.position, cam.transform.rotation, head.transform);
             _actions = GetComponent<PlayerActions>();
             _actions.camera = thisCam.GetComponent<Camera>();
             canvas = thisCam.gameObject.transform.GetChild(0).gameObject;
             useTextobj = canvas.transform.GetChild(0).gameObject;
             useText= useTextobj.GetComponent<TMP_Text>();
             _actions.useText = useText;
             _actions.ready = true;
        }
       
    
    }
}
