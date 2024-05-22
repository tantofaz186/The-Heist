using UnityEngine;

[DefaultExecutionOrder(-2)]
public class MonoBehaviourSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get;  set; }

   
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
   
}