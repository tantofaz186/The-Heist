using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverTextToDoList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Utilizado para ativar e desativar as checkbox no menu de seleção
    public List<GameObject> child;

    void Start()
    {
        foreach (var child in child)
        {
            child.SetActive(false);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var child in child)
        {
            child.SetActive(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var child in child)
        {
            child.SetActive(false);
        }
    }
}
