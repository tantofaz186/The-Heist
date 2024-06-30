using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLocation : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (other.TryGetComponent(out PickupObject item)) item.CollectRpc();
        }
    }
}