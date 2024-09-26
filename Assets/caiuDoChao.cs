using Unity.Netcode;
using UnityEngine;

public class caiuDoChao : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("cheguei");
        if (other.CompareTag("Player"))
        {
            Debug.Log("cheguei2");
            other.gameObject.transform.position = NetworkManager.Singleton.transform.position;
        }
    }
}
