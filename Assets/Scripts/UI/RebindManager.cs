using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class RebindManager : MonoBehaviour
{
    public static PlayerInputActions playerInputActions;

    private GameObject a;
    private void Awake()
    {
        if(playerInputActions == null)
        {
            playerInputActions = new PlayerInputActions();
        }
    }
    

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText)
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
               DoRebind(action, firstPartIndex, statusText,true);
           }
           
       }
       else
       {
           DoRebind(action, bindingIndex, statusText,false);
       }
    }
    static void DoRebind(InputAction actionToRebind, int bindingIndex,TMP_Text statusText,bool allCompositeParts)
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
                   DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts);
               }
           }
       });
       rebind.OnCancel(operation =>
       {
           actionToRebind.Enable();
           operation.Dispose();
       });
       rebind.Start();
    }
}
