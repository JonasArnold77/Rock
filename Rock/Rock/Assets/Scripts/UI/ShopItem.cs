using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    public Image Symbol;

    public Button BuyButton;

    public TMP_Text PriceText;

    public List<Image> Outlines = new List<Image>();

    public GameObject UndiscoveredObject;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ClickOnButton());
            
        Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(false));

        //StartCoroutine(InitGame());

        if (BuyButton != null)
        {
            BuyButton.onClick.AddListener(() => Buy());
        }


        
        //GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
        //Equipment.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if (UndiscoveredObject != null && SaveManager.Instance.SkinsDiscovered.Contains(Equipment.name))
        {
            UndiscoveredObject.SetActive(false);
        }
        else if (UndiscoveredObject != null)
        {
            UndiscoveredObject.SetActive(true);
        }

        if (BuyButton != null && Price >= SaveManager.Instance.Money)
        {
            BuyButton.interactable = false;
            BuyButton.GetComponent<Image>().color = Color.grey;
        }
    }

    private void Buy()
    {
        if(InventoryManager.Instance.MoneyAmount >= Price)
        {
            SaveManager.Instance.SkinsUnlocked.Add(Equipment.name);
            SaveManager.Instance.Save();
            BuyButton.gameObject.SetActive(false);
            GetComponent<Button>().interactable = true;
            GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
            Unlocked = true;

            InventoryManager.Instance.MoneyAmount = InventoryManager.Instance.MoneyAmount - Price;
            SaveManager.Instance.Money = SaveManager.Instance.Money - Price;
            SaveManager.Instance.Save();
            Sidebar.Instance.MoneyAmountText.text = "" + InventoryManager.Instance.MoneyAmount;
        }

        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if (UndiscoveredObject != null && SaveManager.Instance.SkinsDiscovered.Contains(Equipment.name))
        {
            UndiscoveredObject.SetActive(false);
        }
        else if (UndiscoveredObject != null)
        {
            UndiscoveredObject.SetActive(true);
        }

        if (PriceText != null)
        {
            PriceText.text = Price.ToString();
        }

        if (BuyButton != null)
        {
            if (SaveManager.Instance.SkinsUnlocked.Contains(Equipment.name))
            {
                BuyButton.gameObject.SetActive(false);
                GetComponent<Button>().interactable = true;
                GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
            }
            else
            {
                BuyButton.gameObject.SetActive(true);
                GetComponent<Button>().interactable = false;
                GetComponent<Image>().color = ShopMenu.Instance.LockedColor;
            }
        }
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
                    if (child.GetComponent<ShopItem>().Unlocked)
                    {
                        child.GetComponent<ShopItem>().Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(false));
                        child.GetComponent<Image>().color = ShopMenu.Instance.UnselectedColor;
                        child.GetComponent<ShopItem>().Equipped = false;
                    } 
                }

                Equipped = true;

                SaveManager.Instance.ActualSkin = Equipment.ToString();
                SaveManager.Instance.Save();

                //Outlines.Select(o => o.gameObject).ToList().ForEach(o => o.SetActive(true));
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

                InventoryManager.Instance.MoneyAmount = InventoryManager.Instance.MoneyAmount - Price;
                SaveManager.Instance.Money = SaveManager.Instance.Money - Price;
                SaveManager.Instance.Save();
                Sidebar.Instance.MoneyAmountText.text = "" + InventoryManager.Instance.MoneyAmount;

                SaveManager.Instance.ActualSkin = Equipment.ToString();
                SaveManager.Instance.Save();
            }
        }
    }
}
