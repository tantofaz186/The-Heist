using UnityEngine;

public class DropLocation : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Item")) return;
        if (other.TryGetComponent(out MeshRenderer meshRenderer))
        {
            if (!meshRenderer.enabled) return;
        } 
        if (other.TryGetComponent(out PickupObject item))
        {
            item.CollectRpc();
        }
    }
}