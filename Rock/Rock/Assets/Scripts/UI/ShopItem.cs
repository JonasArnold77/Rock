using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public List<Image> Outlines = new List<Image>();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ClickOnButton());

        Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(false));
        //GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
        //Equipment.SetActive(false);
    }

    public void SetOutlinesHighlighted()
    {

    }

    public void SetOutlinesNotHighlighted()
    {

    }

    private void ClickOnButton()
    {
        if (Unlocked)
        {
            if (!Equipped)
            {
                if (Equipment == null)
                {
                    foreach (Transform child in ShopMenu.Instance.Content)
                    {
                        if (child.GetComponent<ShopItem>().Equipment != null)
                        {
                            child.GetComponent<ShopItem>().Equipment.SetActive(false);
                        }
                    }

                    SaveManager.Instance.Save();
                    return;
                }

                FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));

                Equipment.SetActive(true);

                foreach (Transform child in ShopMenu.Instance.Content)
                {
                    child.GetComponent<ShopItem>().Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(false));
                    child.GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
                    child.GetComponent<ShopItem>().Equipped = false;
                }

                Equipped = true;

                SaveManager.Instance.ActualSkin = Equipment.ToString();
                SaveManager.Instance.Save();

                Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(true));
                GetComponent<Image>().color = ShopMenu.Instance.SelectedColor;
            }
            else
            {
                //FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));
                //foreach (Transform child in ShopMenu.Instance.Content)
                //{
                //    child.GetComponent<ShopItem>().Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(false));
                //    GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
                //    child.GetComponent<ShopItem>().Equipped = false;
                //}

                //SaveManager.Instance.Save();
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
