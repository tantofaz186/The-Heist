using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : NetworkBehaviour
{
    public static BulletPool instance;

    [SerializeField]
    private List<GameObject> bulletPool = new List<GameObject>();
    
    [SerializeField]
    private List<GameObject> slowAreaPool = new List<GameObject>();
    [SerializeField]
    private int poolSize = 10;

    [SerializeField]
    private GameObject bulletPrefab;
    
    [SerializeField]
    private GameObject slowAreaPrefab;

    private void Awake()
    {
        if (instance == null)
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
        if (!IsServer) return;
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab, transform);
            var instanceNetworkObject = obj.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            bulletPool.Add(obj);
            obj.GetComponent<Bullet>().DeactivateRpc();
            
        }
        
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(slowAreaPrefab, transform);
            var instanceNetworkObject = obj.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            bulletPool.Add(obj);
            obj.GetComponent<SlowArea>().DeactivateRpc();
            
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                bulletPool[i].GetComponent<Bullet>().ActivateRpc();
                bulletPool[i].SetActive(true);

                return bulletPool[i];
            }
        }

        return null;
    }
    
    public GameObject GetSlowArea()
    {
        for (int i = 0; i < slowAreaPool.Count; i++)
        {
            if (!slowAreaPool[i].activeInHierarchy)
            {
                slowAreaPool[i].GetComponent<SlowArea>().ActivateRpc();
                slowAreaPool[i].SetActive(true);

                return slowAreaPool[i];
            }
        }

        return null;
    }
}