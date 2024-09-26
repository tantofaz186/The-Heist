using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestNetcodeUi : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => 
        { 
            NetworkManager.Singleton.StartHost();
            Debug.Log("HOST");
            Hide();
        });
        clientButton.onClick.AddListener(() => 
        { 
            NetworkManager.Singleton.StartClient();
            Debug.Log("CLIENT");
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
