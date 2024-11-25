using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHud : Singleton<InventoryHud>
{
    public Image[] slots = new Image[4];
    public GameObject[] borders = new GameObject[4];
    public Slider weighSlider;
    public Text moneyText;
    public Image weightImage;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        StartCoroutine(Refresh());
    }

    public IEnumerator Refresh()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(3f);
            if (Inventory.Instance == null) continue;
            if (Inventory.Instance.items == null) continue;
            for (int i = 0; i < Inventory.Instance.items.Length; i++)
            {
                if (Inventory.Instance.items[i] == null)
                {
                    slots[i].color = new Color(slots[i].color.r, slots[i].color.g, slots[i].color.b, 0f);
                    slots[i].sprite = null;
                }
                else
                {
                    slots[i].sprite = Inventory.Instance.items[i].itemSprite;
                }
            }
        }
    }

    public void TryRefresh()
    {
        if (Inventory.Instance == null) return;
        if (Inventory.Instance.items == null) return;
        for (int i = 0; i < Inventory.Instance.items.Length; i++)
        {
            if (Inventory.Instance.items[i] == null)
            {
                slots[i].color = new Color(slots[i].color.r, slots[i].color.g, slots[i].color.b, 0f);
                slots[i].sprite = null;
            }
            else
            {
                slots[i].sprite = Inventory.Instance.items[i].itemSprite;
            }
        }
    }

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
        if (weight < 0.34f)
        {
            weightImage.color = new Color(164 / 255f, 164 / 255f, 164 / 255f, 170 / 255f);
        }
        else if (weight > 0.67f)
        {
            weightImage.color = new Color(307 / 255f, 36 / 255f, 31 / 255f, 170 / 255f);
        }
        else
        {
            weightImage.color = new Color(210 / 255f, 187 / 255f, 39 / 255f, 170 / 255f);
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