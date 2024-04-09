using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class CameraLook : NetworkBehaviour
{
    public PlayerInputActions controls;
    
    public float mouseSensitivity = 50f;
    
    private Vector2 mouseLook;
    
    public Transform body;
    


    private float xRotation = 0f;

    private void Start()
    {
       transform.position = body.position;
        
    }

    
    private void Awake()
    {
       
        body = transform.parent;
        controls = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Look()
    {
        
       
             mouseLook = controls.Player.Mouse.ReadValue<Vector2>();
            
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

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    
    
}
