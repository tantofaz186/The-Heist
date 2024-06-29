using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    [SerializeField] protected Item item;
    public abstract void UseItem();

    public virtual void DropItem()
    {
        
    }
}
