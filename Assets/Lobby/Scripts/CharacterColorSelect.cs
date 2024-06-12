using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelect : MonoBehaviour
{
   [SerializeField] private int colorId;
   [SerializeField] private Image image;
   [SerializeField] private GameObject selectedGameObject;

   private void Awake()
   {
       GetComponent<Button>().onClick.AddListener((() =>
       {
           TheHeistGameMultiplayer.Instance.ChangePlayerColor(colorId);
       }));
   }

   private void Start()
   {
       TheHeistGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged;
       image.color = TheHeistGameMultiplayer.Instance.GetPlayerColor(colorId);
       UpdateIsSelected();
   }

   private void TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
   {
       UpdateIsSelected();
   }

   private void UpdateIsSelected()
   {
       if (TheHeistGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
           selectedGameObject.SetActive(true);
       else
           selectedGameObject.SetActive(false);
   }
   private void OnDestroy() {
       TheHeistGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged;
   }
}
