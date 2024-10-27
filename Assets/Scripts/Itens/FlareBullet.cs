using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlareBullet : NetworkBehaviour
{   [SerializeField]float radius = 10f;
    public override void OnNetworkSpawn()
    {
        Invoke(nameof(CallGuard),3f);
    }

    void CallGuard()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,layerMask: LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Enemy>().ChangePatolLocation(transform.position);
        }
    }
    
}
