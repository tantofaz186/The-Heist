using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
   [SerializeField] private Button mainMenuButton;
   [SerializeField] private Button readyButton;
   [SerializeField] private TextMeshProUGUI lobbyNameText;
   [SerializeField] private TextMeshProUGUI lobbyCodeText;
   
   
   private void Awake()
   {
      mainMenuButton.onClick.AddListener(() => 
      {
         TheHeistGameLobby.Instance.LeaveLobby();
         NetworkManager.Singleton.Shutdown();
         Loader.Load(Loader.Scene.MainMenuScene);
      });
      readyButton.onClick.AddListener(() => 
      { 
         CharacterSelectReady.Instance.SetPlayerReady();
      });
   }

   private void Start()
   {
      Lobby lobby = TheHeistGameLobby.Instance.GetLobby();
      
      lobbyNameText.text = lobby.Name;
      lobbyCodeText.text = lobby.LobbyCode;
   }
}
