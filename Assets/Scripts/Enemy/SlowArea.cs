using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SlowArea : NetworkBehaviour
{
    
    

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Movement>().Slow();
        }
    }

  

    private void OnEnable()
    {
        StartCoroutine(nameof(SlowAreaCoroutine));
    }
    
    IEnumerator SlowAreaCoroutine()
    {
        yield return new WaitForSeconds(4);
        DestroyRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void DeactivateRpc()
    {
        gameObject.SetActive(false);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void ActivateRpc()
    {
        gameObject.SetActive(true);
    }
    [Rpc(SendTo.Server)]
    void DestroyRpc()
    {
        DeactivateRpc();
    }
}
