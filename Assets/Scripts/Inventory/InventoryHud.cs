using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : Singleton<InventoryHud>
{
    public Image[] slots = new Image[4];
    public GameObject[] borders = new GameObject[4];
    public Slider weighSlider;
    public Text moneyText;

    public void AddItem(Item item, int itemPos)
    {
        slots[itemPos].sprite = item.itemSprite;
        slots[itemPos].color = new Color(slots[itemPos].color.r, slots[itemPos].color.g, slots[itemPos].color.b, 1f);
    }

    public void RemoveItem(int itemPos)
    {
        Debug.Log($"removed item at pos: {itemPos}");
        slots[itemPos].color = new Color(slots[itemPos].color.r, slots[itemPos].color.g, slots[itemPos].color.b, 0f);
        slots[itemPos].sprite = null;
    }

    public void ChangeWeight()
    {
        weighSlider.value = Inventory.Instance.currentWeight / (float)Inventory.Instance.MaxWeight;
        CheckColor(weighSlider.value);
    }

    void CheckColor(float weight)
    {
        Debug.LogWarning(weighSlider.gameObject);

        Debug.LogWarning(weighSlider.fillRect.gameObject);
        Debug.LogError($"weight: {weight}");
        Image image = weighSlider.fillRect.GetComponent<Image>();
        if (weight < 0.34f)
        {
            image.material.color = new Color(164, 164, 164);
        }
        else if (weight > 0.67f)
        {
            image.material.color = new Color(307, 36, 31);
        }
        else
        {
            image.material.color = new Color(210, 187, 39);
        }
    }

    public void RemoveRelic(Item item)
    {
        weighSlider.value -= item.itemWeight;
        CheckColor(weighSlider.value);
    }

    public void ChangeActiveItem(int current)
    {
        foreach (var VARIABLE in borders)
        {
            VARIABLE.SetActive(false);
        }

        borders[current].SetActive(true);
    }
}