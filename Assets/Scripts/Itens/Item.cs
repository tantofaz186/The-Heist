using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float itemValue;
    public float itemWeight;
    public GameObject itemPrefab;

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
                Instantiate(item.itemPrefab);
            }
        }
    }
}
#endif
    