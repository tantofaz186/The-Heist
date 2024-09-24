using System;
using System.Collections;
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

    [Rpc(SendTo.Server)]
    public void InstantiateBulletsRpc()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab, transform.position, transform.rotation);
            var instanceNetworkObject = obj.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(OwnerClientId);
            obj.SetActive(false);
            AddBulletToPoolRpc(obj);
        }
    }

    [Rpc(SendTo.Everyone)]
    void AddBulletToPoolRpc(GameObject obj)
    {
        bulletPool.Add(obj);
    }
    
    
    public GameObject GetBullet(out int index)
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                index = i;
                return bulletPool[i];
            }
        }
        index = -1;
        return null;
    }

    public GameObject GetBulletByIndex(int index)
    {
        return bulletPool[index];
    }
}
