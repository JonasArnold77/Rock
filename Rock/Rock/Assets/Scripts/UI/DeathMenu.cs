using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class DeathMenu : MonoBehaviour
{
    public static DeathMenu Instance;

    public Button ShopButton;
    public Button RestartButton;
    public Button HardcoreModeButton;

    public Text ScoreText;
    public Text HighscoreText;

    public List<GameObject> ChallengeGameObjects;
    public Transform Content;

    public Text test;

    public Color ActiveColor;
    public Color InactiveColor;

    public Image LevelBarImage;


    private void Start()
    {
        gameObject.SetActive(false);
        RestartButton.onClick.AddListener(() => Respawn());
        ShopButton.onClick.AddListener(() => ShopMenu.Instance.gameObject.SetActive(true));
        ShopButton.onClick.AddListener(() => ShopMenu.Instance.SetCurrentSkin());
        HardcoreModeButton.onClick.AddListener(() => InventoryManager.Instance.ToggleHardcoreMode());
        //test.text = Application.persistentDataPath;
    }

    private void Update()
    {
        HardcoreModeButton.onClick.RemoveAllListeners();

        if (SaveManager.Instance.HardcoreModeOn)
        {
            HardcoreModeButton.GetComponentInChildren<Text>().text = "Normal Mode";
            HardcoreModeButton.GetComponentInChildren<Text>().color = Color.white;
            HardcoreModeButton.onClick.AddListener(() => InventoryManager.Instance.ToggleHardcoreMode());
        }
        else
        {
            HardcoreModeButton.GetComponentInChildren<Text>().text = "Hardcore Mode";
            HardcoreModeButton.GetComponentInChildren<Text>().color = Color.red;

            HardcoreModeButton.onClick.AddListener(() => InventoryManager.Instance.ToggleHardcoreMode());
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    //public IEnumerator FillLevelBarCoroutine(float amount)
    //{
    //    var difLast = InventoryManager.Instance.InitDistance / 20;

    //    if (InventoryManager.Instance.FinalLevel > InventoryManager.Instance.InitLevel)
    //    {
    //        for (int i = 0; i < 20; i++)
    //        {
    //            //yield return new WaitForSeconds();
    //        }
    //    }

    //    var difActual = amount / 20;

    //    for (int i = 0; i < 20; i++)
    //    {
    //        //yield return new WaitForSeconds();
    //    }
    //}


    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void OnEnable()
    {
        if(ChallengeManager.Instance.actualChallengeButton != null)
        {
            ChallengeManager.Instance.actualChallengeButton.SetActualChallenge();
        }


        SaveManager.Instance.CompleteDistance = 0;

        foreach (var c in SaveManager.Instance.Challenges)
        {
            var posIndex = SaveManager.Instance.Challenges.IndexOf(c);
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().Highscore = SaveManager.Instance.ChallengesScore[posIndex];
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().HighscoreText.text = "" + SaveManager.Instance.ChallengesScore[posIndex];
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().Distance = SaveManager.Instance.ChallengeDistance[posIndex];
            DeathMenu.Instance.ChallengeGameObjects.Where(b => b.GetComponent<ChallengeButton>().title == c).FirstOrDefault().GetComponent<ChallengeButton>().DistanceText.text = "" + SaveManager.Instance.ChallengeDistance[posIndex];
            SaveManager.Instance.CompleteDistance = SaveManager.Instance.CompleteDistance + SaveManager.Instance.ChallengeDistance[posIndex];
        }

        if (InventoryManager.Instance.Score > SaveManager.Instance.ChallengesScore[SaveManager.Instance.Challenges.IndexOf(SaveManager.Instance.Challenges.Where(c => c == ChallengeManager.Instance.actualChallengeButton.title).FirstOrDefault())])
        {
            ChallengeManager.Instance.actualChallengeButton.GetComponent<ChallengeButton>().HighscoreText.text = "" + InventoryManager.Instance.Score;
        }

        if (ChallengeManager.Instance.actualChallengeButton != null)
        {
            InventoryManager.Instance.FinalLevel = GetLevelFromDistance(ChallengeManager.Instance.actualChallengeButton.Distance, ChallengeManager.Instance.actualChallengeButton.LevelDistances);
            StartCoroutine(InventoryManager.Instance.AnimateLevelBar(InventoryManager.Instance.InitLevel, InventoryManager.Instance.FinalLevel, InventoryManager.Instance.InitDistance, ChallengeManager.Instance.actualChallengeButton.Distance));
            ChallengeDetailMenu.Instance.HighscoreText.text = "Highscore: " + ChallengeManager.Instance.actualChallengeButton.Highscore + "\nDistance: " + ChallengeManager.Instance.actualChallengeButton.Distance;
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

    public void SetUpChallengeButtons()
    {

        //ChallengeGameObjects.ForEach(b => b.GetComponentsInChildren<Button>(true).ToList().ForEach(b1 => b1.gameObject.SetActive(true)));

        foreach (var c in ChallengeGameObjects)
        {
            if(c.GetComponent<ChallengeButton>().title == ChallengeManager.Instance.actualChallengeButton.title)
            {
                c.GetComponent<Image>().color = ActiveColor;
                //c.GetComponentsInChildren<Button>()[1].gameObject.SetActive(true);
            }
            else
            {
                c.GetComponent<Image>().color = InactiveColor;
                //c.GetComponentsInChildren<Button>()[1].gameObject.SetActive(false);
            }
        }
    }
}
