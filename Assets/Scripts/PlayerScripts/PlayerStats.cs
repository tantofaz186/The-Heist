using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStats : MonoBehaviour
{
    public GameObject dmgGO;
    private Image dmgImg;
    private bool hit;
    public int bagSize;
    public int maxBagSize;
    private bool canPick=false;
    public List<ItemTemp> itemsList= new List<ItemTemp>();
    public ItemTemp itemTmp;
   [SerializeField] Movement movement;
    void Start()
    {
        movement = GetComponent<Movement>();
        dmgGO = GameObject.Find("DamageImage");
        dmgImg = dmgGO.GetComponent<Image>();
        Debug.Log("Start");
    }

    /*private void Update()
    {   
        if (Input.GetKey(KeyCode.G))
        {   
            DropItem();
        }
        if (Input.GetKey(KeyCode.F))
        {   if(canPick==true)
            PickItem(itemTmp);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hit"))
        {
            Debug.Log("Hit");
            Hit();
        }

        /*if (other.CompareTag("Item"))
        {
            canPick = true;
            itemTmp = other.GetComponent<ItemTemp>();
        }*/
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            canPick = false;
        }
    }*/

    void Hit()
    {
        if (!hit)
        {
            StartCoroutine(TakeDamage());
        }
    }

    IEnumerator TakeDamage()
    {    hit = true;
        dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 1f);
        yield return new WaitForSeconds(0.3f);
        dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 0f);
         hit = false;
    }


    /*public void PickItem(ItemTemp item)
    {
        if(bagSize< maxBagSize)
        {
            itemsList.Add(item);
            bagSize++;
        }
    }
    
    public void DropItem()
    {
        if(bagSize>0)
        {
            bagSize -= itemsList[itemsList.Count - 1].weight;
            itemsList.RemoveAt(itemsList.Count - 1);

        }
    }*/
}
