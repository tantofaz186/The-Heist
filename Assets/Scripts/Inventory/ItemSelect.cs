using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemSelect : Singleton<ItemSelect>, IUseAction
{
    public BaseItem itemInHand;

    public int currentItemIndex { get; private set; }

    public void setActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Item1.performed += SelectItem1;
        PlayerActions.Instance.PlayerInputActions.Player.Item2.performed += SelectItem2;
        PlayerActions.Instance.PlayerInputActions.Player.Item3.performed += SelectItem3;
        PlayerActions.Instance.PlayerInputActions.Player.Item4.performed += SelectItem4;
    }

    public void unsetActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Item1.performed -= SelectItem1;
        PlayerActions.Instance.PlayerInputActions.Player.Item2.performed -= SelectItem2;
        PlayerActions.Instance.PlayerInputActions.Player.Item3.performed -= SelectItem3;
        PlayerActions.Instance.PlayerInputActions.Player.Item4.performed -= SelectItem4;
    }

    public bool ready { get; set; }

    void SelectItem1(InputAction.CallbackContext context)
    {
        currentItemIndex = 0;
        UpdateBaseItem();
    }

    void SelectItem2(InputAction.CallbackContext context)
    {
        currentItemIndex = 1;
        UpdateBaseItem();
    }

    void SelectItem3(InputAction.CallbackContext context)
    {
        currentItemIndex = 2;
        UpdateBaseItem();
    }

    void SelectItem4(InputAction.CallbackContext context)
    {
        currentItemIndex = 3;
        UpdateBaseItem();
    }

    void UpdateBaseItem()
    {
        InventoryHud.Instance.ChangeActiveItem(currentItemIndex);
        if (Inventory.Instance.items[currentItemIndex] != null)
        {
            Inventory.Instance.itemsInHand[Inventory.Instance.items[currentItemIndex].prefabIndex].SetActive(true);
            itemInHand = Inventory.Instance.itemsInHand[Inventory.Instance.items[currentItemIndex].prefabIndex].GetComponent<BaseItem>();
        }
        else
        {
            itemInHand = null;
        }
    }

}