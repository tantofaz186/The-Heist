using Unity.Netcode;
using UnityEngine;

public class ResetGameManagers : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
            Destroy((NetworkManager.Singleton.gameObject));
        
        if(TheHeistGameMultiplayer.Instance != null)
            Destroy((TheHeistGameMultiplayer.Instance.gameObject));
        if(TheHeistGameLobby.Instance != null)
            Destroy((TheHeistGameLobby.Instance.gameObject));
    }
}
