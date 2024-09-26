using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        TheHeistGameMultiplayer.Instance.OnFailedToJoinGame += TheHeistGameMultiplayer_OnFailedToJoinGame;
        TheHeistGameLobby.Instance.OnCreateLobbyStarted += TheHeistGameLobby_OnCreateLobbyStarted;
        TheHeistGameLobby.Instance.OnCreateLobbyFailed += TheHeistGameLobby_OnCreateLobbyFailed;
        TheHeistGameLobby.Instance.OnJoinStarted += TheHeistGameLobby_OnJoinStarted;
        TheHeistGameLobby.Instance.OnJoinFailed += TheHeistGameLobby_OnJoinFailed;
        TheHeistGameLobby.Instance.OnQuickJoinFailed += TheHeistGameLobby_OnQuickJoinCodeFailed;
        Hide();
    }

    private void TheHeistGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    
    private void TheHeistGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create Lobby");
    }

    private void TheHeistGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }
    
    private void TheHeistGameLobby_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void TheHeistGameLobby_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join Lobby");
    }

    private void TheHeistGameLobby_OnQuickJoinCodeFailed(object sender, EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join");
    }
    
    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        TheHeistGameMultiplayer.Instance.OnFailedToJoinGame -= TheHeistGameMultiplayer_OnFailedToJoinGame;
        TheHeistGameLobby.Instance.OnCreateLobbyStarted -= TheHeistGameLobby_OnCreateLobbyStarted;
        TheHeistGameLobby.Instance.OnCreateLobbyFailed -= TheHeistGameLobby_OnCreateLobbyFailed;
        TheHeistGameLobby.Instance.OnJoinStarted -= TheHeistGameLobby_OnJoinStarted;
        TheHeistGameLobby.Instance.OnJoinFailed -= TheHeistGameLobby_OnJoinFailed;
        TheHeistGameLobby.Instance.OnQuickJoinFailed -= TheHeistGameLobby_OnQuickJoinCodeFailed;
    }
}
