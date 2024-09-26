using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;


    private void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            TheHeistGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            TheHeistGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
