using Unity.Netcode;
using UnityEngine;

public class AudioPlay : NetworkBehaviour
{
    public AudioSource audioSource;
    
    [Rpc(SendTo.Everyone)]
    public void PlayAudioClientRpc()
    {   
        audioSource.Play();
    }
    
    [Rpc(SendTo.Everyone)]
    public void PauseAudioClientRpc()
    {   
        audioSource.Pause();
    }
}
