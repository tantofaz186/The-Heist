using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerActions : NetworkBehaviour
{
    [SerializeField]
    public TMP_Text useText;

    [SerializeField]
    public Camera _camera;

    [SerializeField]
    float maxDistance = 5f;

    [SerializeField]
    private LayerMask useLayers;

    public bool ready;

    [SerializeField]
    public Image keySprite;

    public Transform drop;
    public PlayerInputActions playerInputActions;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (IsOwner)
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            playerInputActions.Player.Enable();
            playerInputActions.Player.Use.performed += OpenCloseDoor;
        }
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            playerInputActions.Player.Use.performed -= OpenCloseDoor;
        }
    }

    private void OpenCloseDoor(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
        {
            if (hit.collider.TryGetComponent<Door>(out Door door))
            {
                if (door.isOpen.Value)
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

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, maxDistance,
                useLayers))
        {
            if (hit.collider.TryGetComponent(out PickupObject _))
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
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
    }
}