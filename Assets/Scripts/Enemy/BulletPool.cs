using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : NetworkBehaviour
{
    public static BulletPool instance;

    [SerializeField] NetworkVariable<GameObject>[] bulletPool = new NetworkVariable<GameObject>[10];
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

    [Rpc(SendTo.Everyone)]
    public void InstantiateBulletsRpc()
    {
        for (int i = 0; i < poolSize; i++)
        {
            
            var obj = Instantiate(bulletPrefab, transform.position, transform.rotation);
            var instanceNetworkObject = obj.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
            obj.SetActive(false);
            bulletPool[i].Value = obj;
        }
    }
    
    
    
    
    public GameObject GetBullet(out int index)
    {
        for (int i = 0; i < bulletPool.Length; i++)
        {
            if (!bulletPool[i].Value.activeInHierarchy)
            {
                index = i;
                return bulletPool[i].Value;
            }
        }
        index = -1;
        return null;
    }

    public GameObject GetBulletByIndex(int index)
    {
        return bulletPool[index].Value;
    }
}
