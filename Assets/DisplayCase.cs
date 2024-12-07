using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class DisplayCase : NetworkBehaviour, Interactable
{
    NetworkVariable<bool> isOpen = new NetworkVariable<bool>(false);
    NetworkVariable<bool> unlocked = new NetworkVariable<bool>(false);

    public void TryOpenDisplayCase()
    {
        if (!unlocked.Value)
        {
            int index = Inventory.Instance.items.ToList().FindIndex((i) => i != null && i.itemName == "DisplayCaseKey");
            Debug.Log($"index : {index}");
            if (index > -1)
            {
                UnlockServerRpc();
                Debug.Log("Unlocking Display Case");
                Inventory.Instance.RemoveItem(index);
                TryOpenServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void TryOpenServerRpc()
    {
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
            if (renderer.name == "base_reliquias") continue;
            renderer.enabled = false;
        }
    }

    public string getDisplayText()
    {
        return isOpen.Value ? "" : "Open ";
    }

    public void Interact()
    {
        TryOpenDisplayCase();
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