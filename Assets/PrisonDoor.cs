using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrisonDoor : NetworkBehaviour
{
    NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);

    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Use.performed += TryOpenDoor;

    }
    private PlayerInputActions playerInputActions;
    private void OnDisable()
    {
        playerInputActions.Player.Use.performed -= TryOpenDoor;
    }

    private void TryOpenDoor(InputAction.CallbackContext obj)
    {
        // if has lockpick, open, otherwise return
        Camera __camera = null;
        foreach (var pa in FindObjectsByType<PlayerActions>(FindObjectsSortMode.None))
        {
            if (pa.IsOwner)
            {
                __camera = pa._camera;
            }
        }
        if(__camera == null) return;
        Ray ray = __camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 4f))
        {
            if (hit.collider.gameObject != gameObject) return;
            int lockpickIndex = Inventory.Instance.items.ToList().FindIndex((i) => i.itemName == "Lockpick");
            if (lockpickIndex > -1)
            {
                OpenServerRpc();
                Inventory.Instance.RemoveItem(lockpickIndex);
                Prison.instance.ReleasePrisonerRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void OpenServerRpc()
    {
        if (!isOpen.Value)
        {
            StartCoroutine(RotateDoor(90));
            isOpen.Value = true;
            Invoke(nameof(CloseServerRpc), 5f);
        }
    }

    [Rpc(SendTo.Server)]
    public void CloseServerRpc()
    {
        if (isOpen.Value)
        {
            StartCoroutine(RotateDoor(-90));
            isOpen.Value = false;
        }
    }

    private IEnumerator RotateDoor(float angle)
    {
        float time = 0;
        float duration = 1;
        Quaternion startRotation = gameObject.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, angle, 0) * startRotation;
        while (time < duration)
        {
            gameObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.rotation = endRotation;
    }

    #region editor

    #if UNITY_EDITOR
    [CustomEditor(typeof(PrisonDoor))]
    public class PrisionDoorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open"))
            {
                PrisonDoor door = (target as PrisonDoor)!;
                door.OpenServerRpc();
            }

            if (GUILayout.Button("Close"))
            {
                PrisonDoor door = (target as PrisonDoor)!;
                door.CloseServerRpc();
            }
        }
    }
    #endif

    #endregion
}