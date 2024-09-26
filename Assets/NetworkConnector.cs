using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{ 
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private UnityTransport transport;
    private string match = "^(\\d{3})(\\.?)((\\d{1,3})(\\.?)((\\d{1,3})(\\.?)((\\d{1,3})(:?)(\\d{1,5})?)?)?)?(.*)";
    
    private string subtitution = "$1.$4.$7.$10:$12";
    
    public void Start()
    {
        ipInput.onValueChanged.AddListener(OnValueChange);
    }

    private void OnValueChange(string s)
    {
        StartCoroutine(ReplaceRegex());
    }

    private void OnSubmit(string s)
    {
        StartCoroutine(SetIpToConnect());
    }
    
    
    bool runningRegex = false;

    public IEnumerator SetIpToConnect()
    {
        transport.ConnectionData.Address = ipInput.text;
        transport.ConnectionData.Port = 7777; 
        yield break;
    }

    public IEnumerator ReplaceRegex()
    {
        if (runningRegex) yield break;
        runningRegex = true;
        ipInput.text = Regex.Replace(ipInput.text, "[^\\d]", "");
        ipInput.text = Regex.Replace(ipInput.text, match, subtitution);
        ipInput.text = ipInput.text.Trim(':', '.');
        ipInput.stringPosition = ipInput.text.Length;
        runningRegex = false;
    }

    public void OnDestroy()
    {
        ipInput.onValueChanged.RemoveListener(OnValueChange);
    }
}
