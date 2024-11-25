using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TMP_Text playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener( (() =>
        {
            PlayerData playerData = TheHeistGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            TheHeistGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            TheHeistGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        }));
    }

    private void Start()
    {
        TheHeistGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
       UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (TheHeistGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = TheHeistGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            
            playerNameText.text = playerData.playerName.ToString();
            
            playerVisual.SetPlayerColor(TheHeistGameMultiplayer.Instance.GetPlayerMaterial(playerData.colorId));
        }
        else
            Hide();
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        TheHeistGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= TheHeistGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
