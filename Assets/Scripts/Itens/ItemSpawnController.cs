using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnController : Singleton<ItemSpawnController> {
    [SerializeField] private List<Item> itens;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float chanceToSpawn = 0.3f;
    private void Start()
    {
        SpawnItens();
    }

    private void SpawnItens()
    {
        List<Item> spawnedItens = new List<Item>();
        foreach (Transform point in spawnPoints)
        {
            if (Random.value > chanceToSpawn) continue;
            Item itemToSpawn = itens[Random.Range(0, itens.Count)];
            InstantiateAndAddScript(itemToSpawn.itemPrefab, point, itemToSpawn);
            spawnedItens.Add(itemToSpawn);
        }
        //sempre spawna um item
        if (spawnedItens.Count == 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Item itemToSpawn = itens[Random.Range(0, itens.Count)];
            InstantiateAndAddScript(itemToSpawn.itemPrefab, point, itemToSpawn);

        }
    }
    private void InstantiateAndAddScript(GameObject itemToSpawn, Transform point, Item item)
    {
        GameObject spawnedItem = Instantiate(itemToSpawn, point.position, point.rotation);
        spawnedItem.gameObject.AddComponent<ItemBehaviour>();
        item.GetComponent<ItemBehaviour>().item = item;
    }
}
