using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
   [SerializeField] private Button mainMenuButton;
   [SerializeField] private Button createLobbyButton;
   [SerializeField] private Button quickJoinButton;
   [SerializeField] private Button joinCodeButton;
   [SerializeField] private TMP_InputField joinCodeInputField;
   [SerializeField] private TMP_InputField playerNameInputField;
   [SerializeField] private LobbyCreateUI lobbyCreateUI;
   [SerializeField] private Transform lobbyContainer;
   [SerializeField] private Transform lobbyTemplate;

   private void Awake()
   {
      mainMenuButton.onClick.AddListener(() =>
      {
         TheHeistGameLobby.Instance.LeaveLobby();
         Loader.Load(Loader.Scene.MainMenuScene);
      });
      createLobbyButton.onClick.AddListener(() =>
      {
         lobbyCreateUI.Show();
      });
      quickJoinButton.onClick.AddListener(() =>
      {
         TheHeistGameLobby.Instance.QuickJoin();
      });
      joinCodeButton.onClick.AddListener(() =>
      {
         TheHeistGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
      });
      
      lobbyTemplate.gameObject.SetActive(false);
   }

   private void Start()
   {
      playerNameInputField.text = TheHeistGameMultiplayer.Instance.GetPlayerName();
      playerNameInputField.onValueChanged.AddListener( (string newText) =>
      {
         TheHeistGameMultiplayer.Instance.SetPlayerName(newText);
      });
      TheHeistGameLobby.Instance.OnLobbyListChanged += TheHeistGameLobby_OnLobbyListChanged;
      UpdateLobbyList(new List<Lobby>());
   }

   private void TheHeistGameLobby_OnLobbyListChanged(object sender, TheHeistGameLobby.OnLobbyListChangedEventArgs e)
   {
      UpdateLobbyList(e.lobbiesList);
   }

   private void UpdateLobbyList(List<Lobby> lobbyList)
   {
      foreach (Transform child in lobbyContainer)
      {
         if(child == lobbyTemplate) continue;
         Destroy(child.gameObject);
      }

      foreach (Lobby lobby in lobbyList)
      {
         Transform lobbyTransform = Instantiate(lobbyTemplate,lobbyContainer);
         lobbyTransform.gameObject.SetActive(true);
         lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
      }
   }

   private void OnDestroy()
   {
      TheHeistGameLobby.Instance.OnLobbyListChanged -= TheHeistGameLobby_OnLobbyListChanged;
   }
}
