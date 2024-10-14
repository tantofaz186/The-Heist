using Unity.Netcode;
using UnityEngine;

public abstract class BaseItem : NetworkBehaviour
{
    [SerializeField] protected Item item;
    public abstract void UseItem();
    
}
