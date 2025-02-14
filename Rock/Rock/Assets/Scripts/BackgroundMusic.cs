using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    public static BackgroundMusic Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singletonObject = new GameObject("SingletonExample");
                instance = singletonObject.AddComponent<BackgroundMusic>();
                DontDestroyOnLoad(singletonObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Verhindert doppelte Instanzen
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
