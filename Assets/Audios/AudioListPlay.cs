using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioListPlay : NetworkBehaviour
{
   public List<AudioSource> audios;
   
   [Rpc(SendTo.Everyone)]
   public void PlayAudioClientRpc()
   {
      int rnd = Random.Range(0, audios.Count);
      audios[rnd].Play();
   }
   
}
