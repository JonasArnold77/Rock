using CI.QuickSave;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int Highscore;
    public int XpPoints;
    public int Money;
    public string ActualSkin;
    public bool HardcoreModeOn;

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

        if (HardcoreModeOn)
        {
            InventoryManager.Instance.HellPostProcessing.SetActive(true);
            InventoryManager.Instance.NormalPostProcessing.SetActive(false);
        }
        else
        {
            InventoryManager.Instance.HellPostProcessing.SetActive(false);
            InventoryManager.Instance.NormalPostProcessing.SetActive(true);
        }
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
        QuickSaveWriter.Create("Inventory7")
                       .Write("Highscore", Highscore)
                       .Write("XpPoints", XpPoints)
                       .Write("Money", Money)
                       .Write("ActualSkin", ActualSkin)
                       .Write("HardcoreModeOn", HardcoreModeOn)
                       .Commit();

        //Content.text = QuickSaveRaw.LoadString("Inputs.json");
    }

    public void Load()
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, @"QuickSave\Inventory7.json");
        if (!File.Exists(saveFilePath))
        {
            // Wenn das Save-File nicht existiert, Default-Werte setzen und speichern
            SetDefaultValues();
            Save();
        }
        else
        {
            QuickSaveReader.Create("Inventory7")
                       .Read<int>("Highscore", (r) => { Highscore = r; })
                       .Read<int>("XpPoints", (r) => { XpPoints = r; })
                       .Read<int>("Money", (r) => { Money = r; })
                       .Read<string>("ActualSkin", (r) => { ActualSkin = r; })
                       .Read<bool>("HardcoreModeOn", (r) => { HardcoreModeOn = r; });
        }
    }

    private void SetDefaultValues()
    {
        Highscore = 0;
        XpPoints = 0;
        Money = 0; // Beispiel: Startkapital
        ActualSkin = "";
        HardcoreModeOn = false;

        Debug.Log("Default-Werte gesetzt.");
    }
}
