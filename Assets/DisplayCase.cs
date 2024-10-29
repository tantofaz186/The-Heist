using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class DisplayCase : NetworkBehaviour, Interactable, IUseAction
{
    NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
    NetworkVariable<bool> unlocked = new NetworkVariable<bool>(false);

    public void TryOpenDisplayCase(InputAction.CallbackContext obj)
    {
        if (!unlocked.Value)
        {
            int index = Inventory.Instance.items.ToList().FindIndex((i) => i.itemName == "DisplayCaseKey");
            Debug.Log($"index : {index}");
            if (index > -1)
            {
                UnlockServerRpc();
                Debug.Log("Unlocking Display Case");
                Inventory.Instance.RemoveItem(index);
            }
        }

        TryOpenServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void TryOpenServerRpc()
    {
        if (unlocked.Value) return;
        Debug.Log("Opening Display Case");
        DisableDisplayCase();
        isOpen.Value = true;
    }

    // [Rpc(SendTo.Server)]
    // public void CloseServerRpc()
    // {
    //     StartCoroutine(MoveDisplayCase(false));
    //     isOpen.Value = false;
    // }

    [Rpc(SendTo.Server)]
    public void UnlockServerRpc()
    {
        unlocked.Value = true;
    }

    private void DisableDisplayCase()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if(renderer.name == "base_reliquias") continue;
            renderer.enabled = false;
        }
    }

    public string getDisplayText()
    {
        return isOpen.Value ? "" : "Open ";
    }

    public void Interact()
    {
        TryOpenServerRpc();
    }

    public void setActions()
    {
        Debug.Log("Setting actions for Display Case");
        PlayerActions.Instance.PlayerInputActions.Player.Use.performed += TryOpenDisplayCase;
    }

    public void unsetActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Use.performed -= TryOpenDisplayCase;
    }

    #region editor

    #if UNITY_EDITOR
    [CustomEditor(typeof(DisplayCase))]
    public class DisplayCaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Unlock"))
            {
                DisplayCase displayCase = (target as DisplayCase)!;
                displayCase.UnlockServerRpc();
            }

            if (GUILayout.Button("Open"))
            {
                DisplayCase displayCase = (target as DisplayCase)!;
                displayCase.TryOpenServerRpc();
            }

            if (GUILayout.Button("Close"))
            {
                DisplayCase displayCase = (target as DisplayCase)!;
                // displayCase.CloseServerRpc();
            }
        }
    }
    #endif

    #endregion
}