using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class RebindUI : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionReference;
    
    [SerializeField] bool excludeMouse = true;

    [Range(0, 16)] [SerializeField] private int selectedBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayOptions;

    [Header("Binding Info")] 
    [SerializeField] private InputBinding inputBinding;
    int bindingIndex;
    
    string actionName;
    
    [Header("UI Elements")]
   [SerializeField] TMP_Text actionNameText;
   [SerializeField] Button rebindButton;
   [SerializeField]TMP_Text rebindText;
   [SerializeField] Button resetButton;

   private void OnEnable()
   {
       rebindButton.onClick.AddListener(() => DoRebind());
       resetButton.onClick.AddListener(() => ResetBinding());

       if (inputActionReference != null)
       {   
           GetBindingInfo();
           RebindManager.LoadBindingOverride(actionName);
           UpdateUI();
       }
       
       RebindManager.rebinndComplete += UpdateUI;
       RebindManager.rebindCanceled += UpdateUI;
   }

   private void OnDisable()
   {
         RebindManager.rebinndComplete -= UpdateUI;
         RebindManager.rebindCanceled -= UpdateUI;
   }

   private void OnValidate()
   {
       if (inputActionReference == null) return;
       GetBindingInfo();
         UpdateUI();
   }
    
   private void GetBindingInfo()
   {
       if (inputActionReference.action != null) actionName = inputActionReference.action.name;
       if (inputActionReference.action.bindings.Count > selectedBinding)
       {
           inputBinding = inputActionReference.action.bindings[selectedBinding];
           bindingIndex = selectedBinding;
       }
   }
   private void UpdateUI()
   {
       if(actionNameText != null) actionNameText.text = actionName;
       if (rebindText != null)
       {
           if(Application.isPlaying)
           {
               rebindText.text = RebindManager.GetBindingName(actionName, bindingIndex);
           }
           else
           {
               rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
           }
       }
   }

   void DoRebind()
   {
       RebindManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse);
   }

   private void ResetBinding()
   {
       RebindManager.ResetBind(actionName, bindingIndex);
       UpdateUI();
   }
   
}