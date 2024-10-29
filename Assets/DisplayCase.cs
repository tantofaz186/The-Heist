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
    [SerializeField]
    Transform displayCaseDoorTransform;
    private void TryOpenDisplayCase(InputAction.CallbackContext obj)
    {
        if (!unlocked.Value)
        {
            int index = Inventory.Instance.items.ToList().FindIndex((i) => i.itemName == "DisplayCaseKey");
            if (index > -1)
            {
                UnlockServerRpc();
                Inventory.Instance.RemoveItem(index);
            }
        }
        TryOpenServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void TryOpenServerRpc()
    {
        if(!unlocked.Value) return;
        StartCoroutine(MoveDisplayCase(true));
        isOpen.Value = true;
        Invoke(nameof(CloseServerRpc), 5f);
    }

    [Rpc(SendTo.Server)]
    public void CloseServerRpc()
    {
        StartCoroutine(MoveDisplayCase(false));
        isOpen.Value = false;
    }

    [Rpc(SendTo.Server)]
    public void UnlockServerRpc()
    {
        unlocked.Value = true;
    }

    
    private IEnumerator MoveDisplayCase(bool open)
    {
        float time = 0;
        float duration = 1;
        
        Vector3 openedPosition = displayCaseDoorTransform.position;
        openedPosition.y = displayCaseDoorTransform.localScale.y;
        Vector3 closedPosition = displayCaseDoorTransform.position;
        closedPosition.y = 0;
        while (time < duration)
        {
            gameObject.transform.position = Vector3.Lerp(openedPosition, closedPosition,  (open ? 0 : 1)- time / duration);
            time += Time.deltaTime;
            yield return null;
        } 
    }

    public string getDisplayText()
    {
        return isOpen.Value ? "Close " : "Open ";
    }

    public void Interact()
    {
        if (isOpen.Value)
        {
            CloseServerRpc();
        }
        else
        {
            TryOpenServerRpc();
        }
    }

    public void setActions()
    {
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
                displayCase.CloseServerRpc();
            }
        }
    }
    #endif

    #endregion
}