using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sidebar : MonoBehaviour
{
    public static Sidebar Instance;
    public TMP_Text MoonAmountText;
    public TMP_Text MoneyAmountText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
