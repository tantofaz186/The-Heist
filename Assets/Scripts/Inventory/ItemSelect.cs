using Unity.Netcode;
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
        PlayerActions.Instance.PlayerInputActions.Player.Click.performed += UseItem;
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

   public  void UpdateBaseItem()
    {
        InventoryHud.Instance.ChangeActiveItem(currentItemIndex);
       
        if (Inventory.Instance.items[currentItemIndex] != null)
        {    int currentItem = Inventory.Instance.items[currentItemIndex].prefabIndex;
            itemInHand = Inventory.Instance.itemsInHand[Inventory.Instance.items[currentItemIndex].prefabIndex].GetComponent<BaseItem>();
            ActivateItemRpc(currentItem,NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            itemInHand = null;
            ActivateItemRpc(-1,NetworkManager.Singleton.LocalClientId);
        }
        
    }

    [Rpc(SendTo.Everyone)]
  void  ActivateItemRpc(int current,ulong playerID)
  {
      var inventory = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.GetComponent<Inventory>();
      for (int i = 0; i < inventory.itemsInHand.Length; i++)
      {   if( inventory.itemsInHand[i]!=null)  
          inventory.itemsInHand[i].SetActive(false);
      }
      if (current != -1)
      {
          inventory.itemsInHand[current].SetActive(true);
      }
  }
        
    

}