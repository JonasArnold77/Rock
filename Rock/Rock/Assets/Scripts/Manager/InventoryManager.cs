using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<GameObject> SkinList = new List<GameObject>();

    public PlayerMovement _PlayerMovement;

    public List<Equipment> _EquipmentList = new List<Equipment>();

    public int MoneyAmount;
    
    public Color JumpingColor;
    public Color MagneticColor;

    public Color OrangeColor;

    public bool IsHardcoreMode;

    private int counter = 0; // Der Zähler
    private float nextThreshold = 5f; // Nächster Schwellenwert auf der X-Achse

    public Text HighscoreTicker;
         
    public int Score;

    public GameObject NormalPostProcessing;
    public GameObject HellPostProcessing;

    public Color hellColorBlue;

    public bool GodMode;

    public Color HardcoreRed;
    public Color HardcoreYellow;

    public Color NormalRed;
    public Color NormalBlue;

    public List<int> LevelDistance = new List<int>();

    public int InitDistance;
    public int InitLevel;
    public int FinalLevel;

    public int ActualAmountOfMedals;

    public string challengeString;
    public int index;
    public string actualChallenge;
    public CameraChallengeButton actualChallengeButton;
    public int actualScore;
    public PlayerMovement player;

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        nextThreshold = FindObjectOfType<PlayerMovement>().transform.position.x + nextThreshold;
        _PlayerMovement = FindObjectOfType<PlayerMovement>();

        StartCoroutine(InitGame());


        Application.targetFrameRate = 60; // oder 120 auf High-End-Geräten

    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        challengeString = ChallengeManager.Instance.FindSaveByChallengeName(SaveManager.Instance.CameraChallengesStrings, ChallengeManager.Instance.actualChallengeButton.title);
        index = SaveManager.Instance.CameraChallengesStrings.IndexOf(challengeString);
        actualChallenge = (string)ChallengeManager.Instance.GetSaveParameter(SaveManager.Instance.CameraChallengesStrings[index], "challenge");
        actualChallengeButton = ChallengeDetailMenu.Instance.CameraChallengeButtons.Where(c => c.challengeName == actualChallenge).FirstOrDefault();

        if (actualChallenge != "Normal")
        {
            actualScore = ChallengeManager.Instance.actualChallengeButton.CameraChallengeHighscores[actualChallengeButton.Index - 1];
        }
        
        player = FindObjectOfType<PlayerMovement>();

        MoneyAmount = SaveManager.Instance.Money;
        Sidebar.Instance.MoneyAmountText.text = "" + MoneyAmount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShopMenu.Instance.gameObject.SetActive(true);
        }

        if(player == null)
        {
            return;
        }

        // Aktuelle Position des Objekts auf der X-Achse
        float currentX = player.transform.position.x;

        // Prüfen, ob der aktuelle Schwellenwert überschritten wurde
        if (currentX >= nextThreshold && !_PlayerMovement.IsDead)
        {
            counter++; // Zähler um 1 erhöhen
            Debug.Log("Zähler: " + counter);

            Score++;

            HighscoreTicker.text = Score.ToString() + " Meter";

            // Nächsten Schwellenwert um 5 erhöhe]
            nextThreshold += 5f;

            if (!player.IsDead)
            {
                return;
            }
        }
    }

    public void SetScores()
    {
        if ((string)ChallengeManager.Instance.GetSaveParameter(SaveManager.Instance.CameraChallengesStrings[index], "challenge") != "Normal")
        {
            if (Score >= actualScore)
            {
                //Do things after Camera Challenge done
                var actualList = (int[])ChallengeManager.Instance.GetSaveParameter(SaveManager.Instance.CameraChallengesStrings[index], "completed");
                actualList[actualChallengeButton.Index - 1] = 1;
                SaveManager.Instance.CameraChallengesStrings[index] = ChallengeManager.Instance.UpdateCompletedChallenges(SaveManager.Instance.CameraChallengesStrings[index], actualList.ToArray());
                
                bool allDone = true;

                foreach (var a in actualList.ToList())
                {
                    if(a == 0)
                    {
                        allDone = false;
                    }
                }


                if (!SaveManager.Instance.SkinsDiscovered.Contains(ChallengeManager.Instance.actualChallengeButton._shopItem.Equipment.name) && allDone)
                {
                    SaveManager.Instance.SkinsDiscovered.Add(ChallengeManager.Instance.actualChallengeButton._shopItem.Equipment.name);
                }

                SaveManager.Instance.Save();
            }
        }

        SaveManager.Instance.ChallengeDistance[SaveManager.Instance.Challenges.IndexOf(SaveManager.Instance.Challenges.Where(c => c == ChallengeManager.Instance.actualChallengeButton.title).FirstOrDefault())]++;
        SaveManager.Instance.Save();


        if (Score > SaveManager.Instance.Highscore)
        {
            SaveManager.Instance.Highscore = Score;
        }

        if (Score > SaveManager.Instance.ChallengesScore[SaveManager.Instance.Challenges.IndexOf(SaveManager.Instance.Challenges.Where(c => c == ChallengeManager.Instance.actualChallengeButton.title).FirstOrDefault())])
        {
            SaveManager.Instance.ChallengesScore[SaveManager.Instance.Challenges.IndexOf(SaveManager.Instance.Challenges.Where(c => c == ChallengeManager.Instance.actualChallengeButton.title).FirstOrDefault())] = Score;
            SaveManager.Instance.Save();
        }
    }

    public int GetLevelFromDistance(float finalDistance, List<int> levelDistances)
    {
        for (int i = 0; i < levelDistances.Count; i++)
        {
            if (finalDistance < levelDistances[i])
            {
                return i + 1; // Level beginnt bei 1
            }
        }

        // Falls Spieler weiter ist als alle Distanzen in der Liste → Max-Level erreicht
        return levelDistances.Count + 1;
    }

    public IEnumerator AnimateLevelBar(int initLevel, int finalLevel, int initDistance, int finalDistance, float animationDurationPerLevel = 1f)
    {
     initLevel = GetLevelFromDistance(initDistance, ChallengeManager.Instance.actualChallengeButton.LevelDistances);
     finalLevel = GetLevelFromDistance(finalDistance, ChallengeManager.Instance.actualChallengeButton.LevelDistances);
        
    for (int level = initLevel; level <= finalLevel; level++)
    {
        // Start- und Endgrenzen dieses Levels (z. B. 100–250 für Level 2)
        int startIndex = level - 2;
        float levelStart = (startIndex >= 0) ? ChallengeManager.Instance.actualChallengeButton.LevelDistances[startIndex] : 0f;

        ChallengeDetailMenu.Instance.AmountImage.transform.parent.GetComponentInChildren<TMP_Text>().text = "Level " + (level - 1);

            int endIndex = level - 1;
        float levelEnd = (endIndex < ChallengeManager.Instance.actualChallengeButton.LevelDistances.Count) ? ChallengeManager.Instance.actualChallengeButton.LevelDistances[endIndex] : finalDistance;

        // Nur innerhalb dieser Grenzen rechnen:
        float from = Mathf.Clamp01((initDistance - levelStart) / (levelEnd - levelStart));
        float to   = Mathf.Clamp01((finalDistance - levelStart) / (levelEnd - levelStart));

        // Nur beim ersten Level übernehmen wir den echten Startwert
        if (level > initLevel) from = 0f;

        // Nur beim letzten Level übernehmen wir den echten Endwert
        if (level < finalLevel) to = 1f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1;
            float fill = Mathf.Lerp(from, to, t);
                ChallengeDetailMenu.Instance.AmountImage.fillAmount = fill;
            yield return null;
        }

            // Sicherstellen, dass der Endwert wirklich erreicht ist
            ChallengeDetailMenu.Instance.AmountImage.fillAmount = to;

        // Wenn nächstes Level kommt, reset auf 0
        if (level < finalLevel)
        {
            yield return new WaitForSeconds(0.2f); // optional kleine Pause
             ChallengeDetailMenu.Instance.AmountImage.fillAmount = 0f;
        }
    }
    }

    public void ToggleHardcoreMode()
    {
        SaveManager.Instance.HardcoreModeOn = !SaveManager.Instance.HardcoreModeOn;

        if (SaveManager.Instance.HardcoreModeOn)
        {
            HellPostProcessing.SetActive(true);
            NormalPostProcessing.SetActive(false);
        }
        else
        {
            HellPostProcessing.SetActive(false);
            NormalPostProcessing.SetActive(true);
        }

        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

    }
    public void SetActualSkin()
    {
        if(SaveManager.Instance.ActualSkin == "None" || SaveManager.Instance.ActualSkin == "")
        {
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().ForEach(s => s.SetActive(false));
        }
        else
        {
            var x = FindObjectsOfType<Equipment>().ToList();
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().Where(x => x.ToString() == SaveManager.Instance.ActualSkin).FirstOrDefault().SetActive(true);
            FindObjectsOfType<Equipment>().ToList().Select(x => x.gameObject).ToList().Where(x => x.ToString() != SaveManager.Instance.ActualSkin).ToList().ForEach(x => x.SetActive(false));
        }
    }
}