using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Utils;

public class CameraLook : NetworkBehaviour
{
    public float mouseSensitivity = 50f;

    private Vector2 mouseLook;

    public Transform body;
    public Transform head;

    private InputAction mouse;

    private float xRotation = 0f;
    private PlayerInputActions playerInputActions;

    private void Start()
    {
        transform.position = head.position;
        mouse = playerInputActions.Player.Mouse;
    }

    private void Awake()
    {
        head = transform.parent;
        body = head.transform.parent;
        playerInputActions = ActionManager.Instance.playerInputActions;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Look()
    {
        mouseLook = mouse.ReadValue<Vector2>();

        float mouseX = mouseLook.x * mouseSensitivity * Time.deltaTime * SaveSystem.Settings.mouseSensitivity;
        float mouseY = mouseLook.y * mouseSensitivity * Time.deltaTime * SaveSystem.Settings.mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        body.Rotate(Vector3.up * mouseX);
    }

    void Update()
    {
        Look();
    }
}