using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EndGame : NetworkBehaviour, Interactable
{
    public NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false);

    public string getDisplayText()
    {
        return "End Game \"E\"";
    }

    void Start()
    {
        startPosition = transform.localPosition;
    }

    [Rpc(SendTo.Server)]
    void EndGameServerRpc()
    {
        isPressed.Value = true;
        Timer.instance.ChangeTimeRpc(10f);
        gameObject.layer = 0;
        StartCoroutine(AnimateButton());
    }

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private IEnumerator AnimateButton()
    {
        var time = 0f;
        var totalTime = 0.4f;
        targetPosition = startPosition + transform.forward * 0.06f;
        while (time < totalTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / totalTime);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void Interact()
    {
        if (!isPressed.Value)
        {
            EndGameServerRpc();
        }
    }
}