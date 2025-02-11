using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public static StartMenu Instance;
    public Text text;

    private void Awake()
    {
        Instance = this;
    }
}
