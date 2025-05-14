using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    public static BackgroundMusic Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Verhindert Duplikate
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
