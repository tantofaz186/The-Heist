using UnityEngine;

[DefaultExecutionOrder(-2)]
public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get;  set; }

    #if UNITY_EDITOR
    public virtual void OnValidate()
    {
        if (Instance == null) Instance = this as T;
    }
    #else
    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endif
}