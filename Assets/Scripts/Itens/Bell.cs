using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bell : BaseItem
{   NetworkVariable<bool> used = new NetworkVariable<bool>(false);
    [SerializeField] float radius = 10f;
    public override void UseItem()
    {
        if(!used.Value)
        {
            RingBell();
        }
    }

    private void RingBell()
    {
        used.Value = true;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,layerMask: LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Enemy>().ChangePatolLocation(transform.position);
        }
    }
}
