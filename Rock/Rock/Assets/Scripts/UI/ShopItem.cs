using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public GameObject Equipment;
    public Sprite Image;
    public int Price;
    public bool Unlocked;
    public bool Equipped;
    public string EquipmentName;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ClickOnButton());
        //Equipment.SetActive(false);
    }

    private void ClickOnButton()
    {
        if (Unlocked)
        {
            if (!Equipped)
            {
                FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));
                Equipment.SetActive(true);

                foreach (Transform child in ShopMenu.Instance.Content)
                {
                    child.GetComponent<ShopItem>().GetComponent<Button>().image.color = ShopMenu.Instance.UnselectedColor;
                    child.GetComponent<ShopItem>().Equipped = false;
                }

                Equipped = true;

                SaveManager.Instance.ActualSkin = Equipment.ToString();
                SaveManager.Instance.Save();

                GetComponent<Button>().image.color = ShopMenu.Instance.SelectedColor;
            }
            else
            {
                FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));
                foreach (Transform child in ShopMenu.Instance.Content)
                {
                    child.GetComponent<ShopItem>().GetComponent<Button>().image.color = ShopMenu.Instance.UnselectedColor;
                    child.GetComponent<ShopItem>().Equipped = false;
                }

                SaveManager.Instance.ActualSkin = "None";
                SaveManager.Instance.Save();
            }
        }
        else
        {
            if (InventoryManager.Instance.MoneyAmount >= Price)
            {
                Unlocked = true;
                Equipped = true;

                SaveManager.Instance.ActualSkin = Equipment.ToString();
                SaveManager.Instance.Save();
            }
        }
    }
}
