using Unity.Netcode;
using UnityEngine;

public class Bullet :NetworkBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
         DestroyRpc();
    }

    [Rpc(SendTo.Server)]
    void DestroyRpc()
    {
        DeactivateRpc();
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
}
