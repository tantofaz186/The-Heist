using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : NetworkBehaviour
{
    public static BulletPool instance;

    [SerializeField] private List<GameObject> bulletPool = new List<GameObject>();
    [SerializeField] private int poolSize = 10;
    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab, transform.position, transform.rotation);
            if (IsServer)
            {
                var instanceNetworkObject = obj.GetComponent<NetworkObject>();
                instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
            }
            obj.SetActive(false);
            bulletPool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                ActivateBulletRpc(i);
                return bulletPool[i];
            }
        }
        return null;
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void ActivateBulletRpc(int i)
    {
        bulletPool[i].SetActive(true);
    }
}
