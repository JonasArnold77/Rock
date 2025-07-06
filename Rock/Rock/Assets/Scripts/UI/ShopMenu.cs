using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public Transform Content;

    public Color SelectedColor;
    public Color UnselectedColor;
    public Color LockedColor;

    public static ShopMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetCurrentSkin();
    }

    public void SetCurrentSkin()
    {
        foreach (Transform child in Content)
        {
            //Debug.Log("Test: " + SaveManager.Instance?.ActualSkin);
            //Debug.Log("Test2: " + InventoryManager.Instance._EquipmentList.FirstOrDefault());
            //Debug.Log("Test3: " + child.GetComponent<ShopItem>().Equipment.ToString());

            if (child != null)
            {
                ShopItem shopItem = child.GetComponent<ShopItem>();
                if (shopItem != null)
                {
                    if (shopItem.Equipment != null)
                    {
                        if (shopItem.Equipment != null && shopItem.Equipment.ToString() == SaveManager.Instance.ActualSkin)
                        {
                            InventoryManager.Instance._EquipmentList.ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));

                            child.GetComponent<ShopItem>().Equipped = true;


                            child.GetComponent<Image>().color = SelectedColor;

                            //EventSystem.current.SetSelectedGameObject(child.GetComponent<Button>().gameObject);

                            SaveManager.Instance.ActualSkin = child.GetComponent<ShopItem>().Equipment.ToString();
                            SaveManager.Instance.Save();

                            //child.GetComponent<ShopItem>().Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(true));
                        }
                        else
                        {
                            child.GetComponent<Image>().color = UnselectedColor;
                        }
                    }
                }
            }
        }

        foreach (Transform child in Content)
        {
            //if(child.GetComponent<ShopItem>().Equipment.ToString() == SaveManager.Instance.ActualSkin)
            //{
            //    return;
            //}

            if (child.GetComponent<ShopItem>().PriceText != null)
            {
                child.GetComponent<ShopItem>().PriceText.text = child.GetComponent<ShopItem>().Price.ToString();
            }

            if (child.GetComponent<ShopItem>().BuyButton != null)
            {
                if (SaveManager.Instance.SkinsUnlocked.Contains(child.GetComponent<ShopItem>().Equipment.name))
                {
                    child.GetComponent<ShopItem>().BuyButton.gameObject.SetActive(false);
                    child.GetComponent<ShopItem>().GetComponent<Button>().interactable = true;
                    child.GetComponent<ShopItem>().GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
                    child.GetComponent<ShopItem>().Unlocked = true;
                }
                else
                {
                    child.GetComponent<ShopItem>().BuyButton.gameObject.SetActive(true);
                    child.GetComponent<ShopItem>().GetComponent<Button>().interactable = false;
                    child.GetComponent<ShopItem>().GetComponent<Image>().color = ShopMenu.Instance.LockedColor;
                    child.GetComponent<ShopItem>().Unlocked = false;
                }
            }
        }
    }
}