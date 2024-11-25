using Unity.Netcode;
using UnityEngine.InputSystem;

public class ItemSelect : NetworkBehaviour, IUseAction
{
    public BaseItem itemInHand;
    public static ItemSelect Instance { get; private set; }
    public int currentItemIndex { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            Instance = this;
        }
        else
        {
            enabled = false;
        }
    }

    public void setActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Item1.performed += SelectItem1;
        PlayerActions.Instance.PlayerInputActions.Player.Item2.performed += SelectItem2;
        PlayerActions.Instance.PlayerInputActions.Player.Item3.performed += SelectItem3;
        PlayerActions.Instance.PlayerInputActions.Player.Item4.performed += SelectItem4;
        PlayerActions.Instance.PlayerInputActions.Player.Click.performed += UseItem;
        PlayerActions.Instance.PlayerInputActions.Player.Use.performed += UpdateBaseItem;
        PlayerActions.Instance.PlayerInputActions.Player.Release.performed += UpdateBaseItem;
    }

    private void UseItem(InputAction.CallbackContext obj)
    {
        if (itemInHand != null)
        {
            itemInHand.UseItem();
        }
    }

    public void unsetActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Item1.performed -= SelectItem1;
        PlayerActions.Instance.PlayerInputActions.Player.Item2.performed -= SelectItem2;
        PlayerActions.Instance.PlayerInputActions.Player.Item3.performed -= SelectItem3;
        PlayerActions.Instance.PlayerInputActions.Player.Item4.performed -= SelectItem4;
        PlayerActions.Instance.PlayerInputActions.Player.Click.performed -= UseItem;
        PlayerActions.Instance.PlayerInputActions.Player.Use.performed -= UpdateBaseItem;
        PlayerActions.Instance.PlayerInputActions.Player.Release.performed -= UpdateBaseItem;
    }

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

    public void UpdateBaseItem()
    {
        InventoryHud.Instance.ChangeActiveItem(currentItemIndex);
        if (itemInHand != null)
        {
            itemInHand.HideItemRpc();
        }
        if (Inventory.Instance.itemsInHand[currentItemIndex] != null)
        {
            itemInHand = Inventory.Instance.itemsInHand[currentItemIndex].GetComponent<BaseItem>();
            itemInHand.ShowItemRpc();
        }
        else
        {
            itemInHand = null;
        }
    }

    public void ConsumeItem()
    {
        UpdateBaseItem();
    }
    private void UpdateBaseItem(InputAction.CallbackContext obj)
    {
        UpdateBaseItem();
    }
}