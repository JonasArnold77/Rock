using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    public Transform Content;

    public Color SelectedColor;
    public Color UnselectedColor;

    public static ShopMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetCurrentSkin()
    {
        foreach (Transform child in Content)
        {
            if(child.GetComponent<ShopItem>().Equipment.ToString() == SaveManager.Instance.ActualSkin) 
            {
                FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));

                child.GetComponent<ShopItem>().Equipped = true;

                SaveManager.Instance.ActualSkin = child.GetComponent<ShopItem>().Equipment.ToString();
                SaveManager.Instance.Save();

                child.GetComponent<ShopItem>().Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(true));
            }
        }
    }
}