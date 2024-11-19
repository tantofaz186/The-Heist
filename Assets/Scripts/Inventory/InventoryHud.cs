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

    public void AddRelic(Item item)
    {
        weighSlider.value += item.itemWeight;
        CheckColor(weighSlider.value);
    }

    void CheckColor(float weight)
    {
        if (weight < Inventory.Instance.MaxWeight / 3f)
        {
            weighSlider.fillRect.GetComponent<Image>().color = new Color(164, 164, 164, 170);
        }
        else if (weight > Inventory.Instance.MaxWeight / 1.5f)
        {
            weighSlider.fillRect.GetComponent<Image>().color = new Color(307, 36, 31, 170);
        }
        else
        {
            weighSlider.fillRect.GetComponent<Image>().color = new Color(210, 187, 39, 170);
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