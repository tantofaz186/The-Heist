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
    private bool canPick = false;

    [SerializeField] Movement movement;

    void Start()
    {
        movement = GetComponent<Movement>();
        dmgGO = GameObject.Find("DamageImage");
        dmgImg = dmgGO.GetComponent<Image>();
        Debug.Log("Start");
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hit"))
        {
            Debug.Log("Hit");
            Hit();
        }



        void Hit()
        {
            if (!hit)
            {
                StartCoroutine(TakeDamage());
            }
        }

        IEnumerator TakeDamage()
        {
            hit = true;
            dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 1f);
            yield return new WaitForSeconds(0.3f);
            dmgImg.color = new Color(dmgImg.color.r, dmgImg.color.g, dmgImg.color.b, 0f);
            hit = false;
        }


    }
}
