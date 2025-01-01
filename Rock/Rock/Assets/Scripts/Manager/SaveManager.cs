using CI.QuickSave;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int Highscore;
    public int XpPoints;
    public int Money;
    public string ActualSkin;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var path = Application.persistentDataPath;
        Load();
        FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().ForEach(s => s.SetActive(true));
        InventoryManager.Instance.SetActualSkin();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    public void Save()
    {
        QuickSaveWriter.Create("Inventory")
                       .Write("Highscore", Highscore)
                       .Write("XpPoints", XpPoints)
                       .Write("Money", Money)
                       .Write("ActualSkin", ActualSkin)
                       .Commit();

        //Content.text = QuickSaveRaw.LoadString("Inputs.json");
    }

    public void Load()
    {
        QuickSaveReader.Create("Inventory")
                       .Read<int>("Highscore", (r) => { Highscore = r; })
                       .Read<int>("XpPoints", (r) => { XpPoints = r; })
                       .Read<int>("Money", (r) => { Money = r; })
                       .Read<string>("ActualSkin", (r) => { ActualSkin = r; });
    }
}
