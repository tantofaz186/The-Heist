using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        TheHeistGameMultiplayer.Instance.OnTryingToJoinGame += TheHeistGameMultiplayer_OnTryingToJoinGame;
        TheHeistGameMultiplayer.Instance.OnFailedToJoinGame += TheHeistGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void TheHeistGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void TheHeistGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
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
        TheHeistGameMultiplayer.Instance.OnTryingToJoinGame -= TheHeistGameMultiplayer_OnTryingToJoinGame;
        TheHeistGameMultiplayer.Instance.OnFailedToJoinGame -= TheHeistGameMultiplayer_OnFailedToJoinGame;
    }
}
