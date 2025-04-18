using CI.QuickSave;
using System;
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
    public string LastChunkType;
    public string LastChunk;
    public int CountOfArea;
    public bool TutorialDone;

    public List<string> Challenges;
    public List<int> ChallengesScore;

    public string ActualChallenge;

    public List<string> HardcoreLevelList;

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

        //if(CountOfArea >= 5)
        //{
        //    LevelManager.Instance.countOfArea = 7;
        //}
        //else
        //{
        //    LevelManager.Instance.countOfArea = CountOfArea + 2;
        //}


        ChallengeManager.Instance.actualChallengeButton = DeathMenu.Instance.ChallengeGameObjects.Where(c => c.GetComponent<ChallengeButton>().title == ActualChallenge).FirstOrDefault().GetComponent<ChallengeButton>();
        DeathMenu.Instance.SetUpChallengeButtons();
        DeathMenu.Instance.ChallengeGameObjects.Where(c => c.GetComponent<ChallengeButton>().title == ActualChallenge).FirstOrDefault().GetComponent<ChallengeButton>().ChallengeFunction.Invoke();

        foreach (var c in Challenges)
        {
            var posIndex = Challenges.IndexOf(c);
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().Highscore = ChallengesScore[posIndex];
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().HighscoreText.text = "" + ChallengesScore[posIndex];
        }

        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera")
        {
            FindObjectOfType<FollowPlayer>().transform.SetParent(FindObjectOfType<FollowPlayer>().PlayerTransform);
            FindObjectOfType<FollowPlayer>().ChooseNewTarget();
        }

        if (DeathMenu.Instance.ChallengeGameObjects.Count > Challenges.Count)
        {
            for(int i = 0; i < DeathMenu.Instance.ChallengeGameObjects.Count; i++)
            {
                if(i >= Challenges.Count)
                {
                    Challenges.Add(DeathMenu.Instance.ChallengeGameObjects[i].GetComponent<ChallengeButton>().title);
                    ChallengesScore.Add(DeathMenu.Instance.ChallengeGameObjects[i].GetComponent<ChallengeButton>().Highscore);
                }
            }
        }

        if (HardcoreModeOn)
        {
            InventoryManager.Instance.HellPostProcessing.SetActive(true);
            InventoryManager.Instance.NormalPostProcessing.SetActive(false);

            FindObjectsOfType<FireBall>().ToList().ForEach(f => f.FireHell.enabled = true);
            FindObjectsOfType<FireBall>().ToList().ForEach(f => f.FireNormal.enabled = false);

            LevelManager.Instance.FirstObjectString = LastChunk;

            if(LevelManager.Instance.FirstObjectString != "")
            {
                if (LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().name == LevelManager.Instance.FirstObjectString).FirstOrDefault().GetComponent<Obstacle>().startType == EObstacleType.Middle)
                {
                    LevelManager.Instance.StairSetted = false;
                }
                else
                {
                    LevelManager.Instance.StairSetted = true;
                }
            }
        }
        else
        {
            LevelManager.Instance.countOfArea = UnityEngine.Random.Range(4, 6);

            InventoryManager.Instance.HellPostProcessing.SetActive(false);
            InventoryManager.Instance.NormalPostProcessing.SetActive(true);

            FindObjectsOfType<FireBall>().ToList().ForEach(f => f.FireHell.enabled = false);
            FindObjectsOfType<FireBall>().ToList().ForEach(f => f.FireNormal.enabled = true);

            LevelManager.Instance.actualChunkType = LevelManager.Instance.GetRandomEnumValue<EChunkType>();
        }

        if (TutorialDone)
        {
            TutorialMenu.Instance.gameObject.SetActive(false);
        }
        else
        {
            TutorialMenu.Instance.gameObject.SetActive(true);
        }

        LevelManager.Instance.GameIsInitialized = true;
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
        if (LevelChunkManager.Instance.TestMode)
        {
            LastChunk = "";
        }

        QuickSaveWriter.Create("Inventory26")
                       .Write("Highscore", Highscore)
                       .Write("XpPoints", XpPoints)
                       .Write("Money", Money)
                       .Write("ActualSkin", ActualSkin)
                       .Write("LastChunkType", LastChunkType)
                       .Write("LastChunk", LastChunk)
                       .Write("HardcoreModeOn", HardcoreModeOn)
                       .Write("CountOfArea", CountOfArea)
                       .Write("TutorialDone", TutorialDone)
                       .Write("Challenges", Challenges)
                       .Write("Highscores", ChallengesScore)
                       .Write("ActualChallenge", ActualChallenge)
                       .Commit();

        //Content.text = QuickSaveRaw.LoadString("Inputs.json");
    }

    public void Load()
    {

#if UNITY_ANDROID
        string saveFilePath = Path.Combine(Application.persistentDataPath, @"QuickSave/Inventory26.json");
#elif UNITY_STANDALONE_WIN
        string saveFilePath = Path.Combine(Application.persistentDataPath, @"QuickSave\Inventory26.json");
#elif UNITY_EDITOR
        string saveFilePath = Path.Combine(Application.persistentDataPath, @"QuickSave\Inventory26.json");
#endif


        if (!File.Exists(saveFilePath))
        {
            // Wenn das Save-File nicht existiert, Default-Werte setzen und speichern
            SetDefaultValues();
            Save();

            //DeathMenu.Instance.gameObject.SetActive(true);
            //DeathMenu.Instance.test.text = "Save File Existiert Nicht";
        }
        else
        {
            QuickSaveReader.Create("Inventory26")
                       .Read<int>("Highscore", (r) => { Highscore = r; })
                       .Read<int>("XpPoints", (r) => { XpPoints = r; })
                       .Read<int>("Money", (r) => { Money = r; })
                       .Read<string>("ActualSkin", (r) => { ActualSkin = r; })
                       .Read<string>("LastChunkType", (r) => { LastChunkType = r; })
                       .Read<string>("LastChunk", (r) => { LastChunk = r; })
                       .Read<bool>("HardcoreModeOn", (r) => { HardcoreModeOn = r; })
                       .Read<int>("CountOfArea", (r) => { CountOfArea = r; })
                       .Read<bool>("TutorialDone", (r) => { TutorialDone = r; })
                       .Read<List<string>>("Challenges", (r) => { Challenges = r; })
                       .Read<List<int>>("Highscores", (r) => { ChallengesScore = r; })
                       .Read<string>("ActualChallenge", (r) => { ActualChallenge = r; });
            //DeathMenu.Instance.gameObject.SetActive(true);
            //DeathMenu.Instance.test.text = "Save File Existiert";
        }
    }

    private void SetDefaultValues()
    {
        Highscore = 0;
        XpPoints = 0;
        Money = 0; // Beispiel: Startkapital
        ActualSkin = "";
        HardcoreModeOn = false;
        TutorialDone = false;
        CountOfArea = 4;
        LastChunk = "";
        Challenges = DeathMenu.Instance.ChallengeGameObjects.Select(c => c.GetComponent<ChallengeButton>().title).ToList();
        ChallengesScore = DeathMenu.Instance.ChallengeGameObjects.Select(c => c.GetComponent<ChallengeButton>().Highscore).ToList();
        ActualChallenge = "Normal";
        Debug.Log("Default-Werte gesetzt.");
    }
}
