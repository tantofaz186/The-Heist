using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class PlayerActions : SingletonPerPlayer<PlayerActions>
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

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }
    public void setActions()
    {
        Instance.playerInputActions.Player.Move.performed += Movement.Instance.Move;
        Instance.playerInputActions.Player.Run.performed += Movement.Instance.Run;
        Instance.playerInputActions.Player.Jump.performed += Movement.Instance.Jump;
        Instance.playerInputActions.Player.Crouch.performed += Movement.Instance.Crouch;
    }
    public void unsetActions()
    {
        Instance.playerInputActions.Player.Move.performed -= Movement.Instance.Move;
        Instance.playerInputActions.Player.Run.performed -= Movement.Instance.Run;
        Instance.playerInputActions.Player.Jump.performed -= Movement.Instance.Jump;
        Instance.playerInputActions.Player.Crouch.performed -= Movement.Instance.Crouch;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Instance.playerInputActions = new PlayerInputActions();
        Instance.playerInputActions.Enable();
        Instance.playerInputActions.Player.Enable();
        Instance.playerInputActions.Player.Use.performed += PLayerInteract;
        setActions();

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            playerInputActions.Player.Use.performed -= PLayerInteract;
            unsetActions();
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