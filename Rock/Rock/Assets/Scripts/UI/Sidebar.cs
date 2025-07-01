using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidebar : MonoBehaviour
{
    public static Sidebar Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
