using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class CameraLook : NetworkBehaviour
{
    
    public float mouseSensitivity = 50f;
    
    private Vector2 mouseLook;
    
    public Transform body;
    public Transform head;
    
    


    private float xRotation = 0f;

    private void Start()
    {
       transform.position = head.position;
        
    }

    
    private void Awake()
    {
        head = transform.parent;
        body = head.transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Look()
    {
        
       
             mouseLook = PlayerActionsSingleton.Instance.PlayerInputActions.Player.Mouse.ReadValue<Vector2>();
            
                    float mouseX = mouseLook.x * mouseSensitivity * Time.deltaTime;
                    float mouseY = mouseLook.y * mouseSensitivity * Time.deltaTime;
            
                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            
                    transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
                    body.Rotate(Vector3.up*mouseX);
        
        
       
    }
    
    void Update()
    {   
        Look();
    }
    
}
