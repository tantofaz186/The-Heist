using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeMusic : MonoBehaviour
{
    [SerializeField]AudioClip music;
    void Start()
    {
        MusicController.instance.currentMusic.clip = music;
        MusicController.instance.currentMusic.Play();
    }

    
}
