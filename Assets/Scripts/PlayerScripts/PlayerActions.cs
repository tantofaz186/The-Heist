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
    private PlayerInputActions playerInputActions;
    public PlayerInputActions PlayerInputActions => playerInputActions;

    public static PlayerActions Instance;
    private void Awake() { }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Use.performed += PLayerInteract;

        foreach (NetworkBehaviour networkBehaviour in FindObjectsByType<NetworkBehaviour>(FindObjectsSortMode.None))
        {
            if (!networkBehaviour.IsOwner) continue;
            IUseAction action;
            if (networkBehaviour.TryGetComponent<IUseAction>(out action)) action.setActions();
        }

        foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            IUseAction action;
            if (behaviour.TryGetComponent<IUseAction>(out action)) action.setActions();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            foreach (NetworkBehaviour networkBehaviour in FindObjectsByType<NetworkBehaviour>(FindObjectsSortMode.None))
            {
                if (networkBehaviour.IsOwner)
                {
                    IUseAction action;
                    if (networkBehaviour.TryGetComponent<IUseAction>(out action)) action.unsetActions();
                }
            }

            foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                IUseAction action;
                if (behaviour.TryGetComponent<IUseAction>(out action)) action.unsetActions();
            }

            playerInputActions.Player.Use.performed -= PLayerInteract;
            playerInputActions.Player.Disable();
        }
    }

    private void PLayerInteract(InputAction.CallbackContext obj)
    {
        if (!IsOwner) return;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, maxDistance, useLayers))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!ready)
        {
            return;
        }

        RaycastToInteractable();
    }

    private void RaycastToInteractable()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, maxDistance,
                useLayers))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                useText.SetText(interactable.getDisplayText());
                useText.gameObject.SetActive(true);
            }
            else
                useText.gameObject.SetActive(false);
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
    }
}