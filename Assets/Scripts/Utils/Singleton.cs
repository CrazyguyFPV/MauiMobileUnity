using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object _lock = new object();
    private static T _instance;

    public static T Instance
    {
        get
        {
            // Lock preventing from simultaneous access by multiple sources.
            lock (_lock)
            {
                // If it's the first time accessing this singleton Instance, _instance will always be null
                // Searching for an active instance of type T in the scene.
                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            _instance = null;
    }

}

