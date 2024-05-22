using System;
using Unity.Netcode;
using UnityEngine;

//só derivar o objeto Singleton dessa classe, ou de SingletonPersistent se não for pra destruir on load
[DefaultExecutionOrder(-1)]
public class SingletonNetwork<T> : NetworkBehaviour
    where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            T[] objs = FindObjectsOfType(typeof(T)) as T[];
            objs ??= Array.Empty<T>();
            switch (objs.Length)
            {
                case <= 0:
                    Debug.LogError("There is no " + typeof(T).Name + " in the scene.");
                    break;
                case > 1:
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    _instance = objs[0];
                    break;
                case > 0:
                    _instance = objs[0];
                    break;
            }

            return _instance;
        }
    }
}