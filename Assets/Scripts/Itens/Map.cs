using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class Map : NetworkBehaviour,Interactable
{
    [SerializeField] private List<Image> mapFoundPartsImages = new List<Image>();
    [SerializeField] private GameObject fullMap;
    public void Start()
    {
        fullMap = GameObject.FindGameObjectWithTag("Minimap");
        fullMap.GetComponent<Image>().material.SetFloat("_AlphaControl", 0.0f);
        var go = GameObject.FindGameObjectWithTag("MiniMapFoundParts");
        foreach (Transform child in go.transform.GetComponentsInChildren<Transform>())
        {
            if(child != go.transform)
            {
                mapFoundPartsImages.Add(child.GetComponent<Image>());
                child.GetComponent<CanvasGroup>().alpha = 0.0f;
            }
        }
    }

    public string getDisplayText()
    {
        return "Pick Map";
    }

    public void Interact()
    {
       PickMapRpc();
    }
    [Rpc(SendTo.Everyone)]
    void PickMapEveryoneRpc()
    {
        var playerStats = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerStats>();
        playerStats.mapPieces++;
        mapFoundPartsImages[playerStats.mapPieces-1].GetComponent<CanvasGroup>().alpha = 1.0f;
        if(playerStats.mapPieces >=3)
        {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                foreach (var piece in mapFoundPartsImages)
                {
                    seq.Join(piece.DOFade(0.0f, 2.0f));
                }
            });
            seq.AppendInterval(5f);
            seq.Append(fullMap.GetComponent<Image>().material.DOFloat(1.0f,"_AlphaControl", 1.0f));
            seq.Play();
        }
        this.gameObject.SetActive(false);
    }
    
    [Rpc(SendTo.Server)]
   void PickMapRpc()
    {
            PickMapEveryoneRpc();
    }
}
