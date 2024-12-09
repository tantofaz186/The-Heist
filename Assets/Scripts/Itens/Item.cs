using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{ 
    public bool isRelic;
    public string itemName;
    public Sprite itemSprite;
    public int itemValue;
    public int itemWeight;
    public GameObject itemPrefab;
    public int prefabIndex;
    public ItemType type;
    public enum ItemType
    {   
        Relic,
        Radio,
        Flare,
        Key,
        NightVision,
       
    }

    public override string ToString()
    {
        return itemName;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Spawn Item"))
        {
            Item item = (target as Item)!;
            if(item.itemPrefab != null)
            {
                GameObject obj;
                try
                {
                    obj = Instantiate(item.itemPrefab, NetworkManager.Singleton.LocalClient.PlayerObject.transform);
                }
                catch
                {
                    obj = Instantiate(item.itemPrefab);
                }
                
                if(obj.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                {
                    networkObject.Spawn();
                }
            }
        }
    }
}
#endif
    