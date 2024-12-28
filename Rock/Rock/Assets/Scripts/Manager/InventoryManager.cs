using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<GameObject> SkinList = new List<GameObject>();

    public int MoneyAmount;

    public Color JumpingColor;
    public Color MagneticColor;

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShopMenu.Instance.gameObject.SetActive(true);
        }
    }

    public void SetActualSkin()
    {
        if(SaveManager.Instance.ActualSkin != "None")
        {
            var x = FindObjectsOfType<Equipment>().ToList();
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().Where(x => x.ToString() == SaveManager.Instance.ActualSkin).FirstOrDefault().SetActive(true);
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().Where(x => x.ToString() != SaveManager.Instance.ActualSkin).ToList().ForEach(x=>x.SetActive(false));
        }
        else
        {
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().ForEach(s => s.SetActive(false));
        }
    }
}