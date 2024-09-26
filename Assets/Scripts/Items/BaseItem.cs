using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField] protected Item item;
    public abstract void UseItem();

    public virtual void DropItem()
    {
        
    }
}
