using CI.QuickSave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int Highscore;
    public int XpPoints;
    public int Money;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Load();
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
                       .Commit();

        //Content.text = QuickSaveRaw.LoadString("Inputs.json");
    }

    public void Load()
    {
        QuickSaveReader.Create("Inventory")
                       .Read<int>("Highscore", (r) => { Highscore = r; })
                       .Read<int>("XpPoints", (r) => { XpPoints = r; })
                       .Read<int>("Money", (r) => { Money = r; });
    }
}
