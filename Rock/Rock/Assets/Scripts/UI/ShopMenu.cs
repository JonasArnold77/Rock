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
            if(child.GetComponent<ShopItem>().Equipment.ToString() == SaveManager.Instance.ActualSkin) 
            {
                FindObjectsOfType<Equipment>().ToList().Select(e => e.gameObject).ToList().ForEach(e => e.SetActive(false));

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