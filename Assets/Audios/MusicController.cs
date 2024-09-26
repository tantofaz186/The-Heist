using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    public AudioSource currentMusic;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentMusic = GetComponent<AudioSource>();
        
    }

    
}
