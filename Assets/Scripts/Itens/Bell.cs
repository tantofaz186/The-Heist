using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bell : BaseItem
{
    private bool used;
    [SerializeField] float radius = 50f;
    public override void UseItem()
    {
        if(!used)
        {
            RingBell();
        }
    }

    private void RingBell()
    {
       
    Debug.Log("tentei");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,layerMask: LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Enemy>().ChangePatolLocation(transform.position);
            Debug.Log("Chamei");
        }

        used = true;
        Invoke(nameof(ResetUse),3);
    }

    private void ResetUse()
    {
        used = false;
    }
}
