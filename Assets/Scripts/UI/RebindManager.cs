using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-101)]
public class RebindManager : MonoBehaviour
{
    public static PlayerInputActions playerInputActions;

    public static event Action rebinndComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction,int> rebindStarted;
    
    private void Awake()
    {
        if(playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();
        }
    }
    

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
       InputAction action = playerInputActions.asset.FindAction(actionName);
       if (action == null || action.bindings.Count <= bindingIndex)
       {
           Debug.Log("Could not find action or binding index");
           return;
       }
       if(action.bindings[bindingIndex].isComposite)
       {
           var firstPartIndex = bindingIndex + 1;
           if(firstPartIndex<action.bindings.Count && action.bindings[firstPartIndex].isComposite)
           {
               DoRebind(action, firstPartIndex, statusText,true, excludeMouse);
           }
           
       }
       else
       {
           DoRebind(action, bindingIndex, statusText,false, excludeMouse);
       }
    }
    static void DoRebind(InputAction actionToRebind, int bindingIndex,TMP_Text statusText,bool allCompositeParts, bool excludeMouse)
    {
       if(actionToRebind == null||bindingIndex<0) return;
       
       statusText.text = $"Press a {actionToRebind.expectedControlType}";
       
       actionToRebind.Disable();
       var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
       
       rebind.OnComplete(operation =>
       {
           actionToRebind.Enable();
           operation.Dispose();
           if (allCompositeParts)
           {
               var nextBindingIndex = bindingIndex + 1;
               if(nextBindingIndex<actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
               {
                   DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
               }
           }
            SaveBindingOverride(actionToRebind);
              rebinndComplete?.Invoke();
       });
       rebind.OnCancel(operation =>
       {
           actionToRebind.Enable();
           operation.Dispose();
              rebindCanceled?.Invoke();
       });
       
       rebind.WithCancelingThrough("<Keyboard>/escape");

       if (excludeMouse)
       {
           rebind.WithControlsExcluding("Mouse");
       }
       
       rebindStarted?.Invoke(actionToRebind, bindingIndex);
       rebind.Start();
    }
    
    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (playerInputActions == null) playerInputActions = new PlayerInputActions();

        InputAction action = playerInputActions.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap+ action.name + i, action.bindings[i].overridePath);
        }
    }
    
   public static void LoadBindingOverride(string actionName)
    {    
        if(playerInputActions == null) playerInputActions = new PlayerInputActions();
        InputAction action = playerInputActions.asset.FindAction(actionName);
        for(int i = 0; i<action.bindings.Count; i++)
        {
           if(!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
           {
               action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
           }
        }
    }

    public static void ResetBind(string actionName, int bindingIndex)
    {
        InputAction action = playerInputActions.asset.FindAction(actionName);
        
        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding index");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i <action.bindings.Count&& action.bindings[i].isComposite ; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        SaveBindingOverride(action);
    }
    
    
}
