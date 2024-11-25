using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlareBullet : NetworkBehaviour
{   [SerializeField]float radius = 50f;
    public AudioPlay audioPlay;
    public override void OnNetworkSpawn()
    {
        Invoke("CallGuard",3f);
    }

    void CallGuard()
    {       Debug.Log("tentei chamar");
        audioPlay.PlayAudioClientRpc();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,layerMask: LayerMask.GetMask("Enemy"));
        Debug.Log(hitColliders.Length);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Enemy>().ChangePatolLocation(transform.position);
            
        }
    }
    
}
