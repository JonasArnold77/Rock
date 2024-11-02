using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int MoneyAmount;

    private void Awake()
    {
        Instance = this; 
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShopMenu.Instance.gameObject.SetActive(true);
        }
    }
}
