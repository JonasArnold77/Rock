using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public Transform Content;

    public Color SelectedColor;
    public Color UnselectedColor;

    public static ShopMenu Instance;

    private void Start()
    {
        Instance = this;
    }
}