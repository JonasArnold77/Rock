using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ChallengeButton : MonoBehaviour
{
    public UnityEvent ChallengeFunction;
    public string title;
    public int Level;
    public int Highscore;
    public int Distance;
    public Button RestartButton;
    public TMP_Text HighscoreText;
    public TMP_Text DistanceText;

    public int AmountOfMedals;

    public string showingTitle;
    public string description;

    public VideoClip Video;

    public ChallengeType ActualChallengeType;

    public List<int> LevelDistances = new List<int>();

    public GameObject LockObject;

    public string CameraChallengeString;

    public ShopItem _shopItem;

    public List<int> CameraChallengeHighscores;


    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ChallengeFunction.Invoke());
        GetComponent<Button>().onClick.AddListener(() => SetActualChallenge());
        GetComponent<Button>().onClick.AddListener(() => SaveManager.Instance.Save());


    }

    private void Awake()
    {
        SetCameraChallengeString();
    }
    private void OnEnable()
    {
        HighscoreText.text = "" + Highscore;
    }

    public IEnumerator SetCameraChallengeString()
    {
        yield return new WaitUntil(() => LevelManager.Instance.InitIsDone);
        CameraChallengeString = ChallengeManager.Instance.FindSaveByChallengeName(SaveManager.Instance.CameraChallengesStrings,title);
    }

    public void SetActualChallenge()
    {
        ChallengeManager.Instance.actualChallengeButton = this;
        SaveManager.Instance.ActualChallenge = title;
        DeathMenu.Instance.SetUpChallengeButtons();

        

        SetUpChallengeDetailMenu();
        //GetComponentsInChildren<Button>().Where(b => !b.Equals(GetComponent<Button>())).FirstOrDefault().gameObject.SetActive(true);
        //GetComponentsInChildren<Button>()[1].onClick.AddListener(() => Respawn());
        HighscoreText.text = "" + Highscore;
    }

    public void SetUpChallengeDetailMenu()
    {
        ChallengeDetailMenu.Instance.gameObject.SetActive(true);
        ChallengeDetailMenu.Instance.HeadlineText.text = showingTitle;
        ChallengeDetailMenu.Instance.DiscriptionText.text = description;
        ChallengeDetailMenu.Instance.GetComponentInChildren<VideoPlayer>().clip = Video;

        ChallengeDetailMenu.Instance.HighscoreText.text = "Highscore: " + Highscore + "\nDistance: " + Distance;

        if(Highscore >= LevelDistances[0])
        {
            ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Medal>().ActiveColor;
            ChallengeDetailMenu.Instance.BronceMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Medal>().ActiveTextBoxColor;
        }
        else
        {
            ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Medal>().InActiveColor;
            ChallengeDetailMenu.Instance.BronceMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.BronceMedalGO.GetComponent<Medal>().InActiveTextBoxColor;
        }

        if (Highscore >= LevelDistances[1])
        {
            ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Medal>().ActiveColor;
            ChallengeDetailMenu.Instance.SilverMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Medal>().ActiveTextBoxColor;
        }
        else
        {
            ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Medal>().InActiveColor;
            ChallengeDetailMenu.Instance.SilverMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.SilverMedalGO.GetComponent<Medal>().InActiveTextBoxColor;
        }

        if (Highscore >= LevelDistances[2])
        {
            ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Medal>().ActiveColor;
            ChallengeDetailMenu.Instance.GoldMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Medal>().ActiveTextBoxColor;
        }
        else
        {
            ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Image>().color = ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Medal>().InActiveColor;
            ChallengeDetailMenu.Instance.GoldMedalGO.transform.GetChild(0).GetComponent<Image>().color = ChallengeDetailMenu.Instance.GoldMedalGO.GetComponent<Medal>().InActiveTextBoxColor;
        }

        string actualCameraChallengeName = (string)ChallengeManager.Instance.GetSaveParameter(ChallengeManager.Instance.FindSaveByChallengeName(SaveManager.Instance.CameraChallengesStrings, ChallengeManager.Instance.actualChallengeButton.title), "challenge");
        var button = ChallengeDetailMenu.Instance.CameraChallengeButtons.Where(c => c.challengeName == actualCameraChallengeName).FirstOrDefault();
        ChallengeDetailMenu.Instance.ActualCameraChallenge(button);

        ChallengeDetailMenu.Instance.Symbol.sprite = _shopItem.Symbol.sprite;
        

        var Level = InventoryManager.Instance.GetLevelFromDistance(Distance, LevelDistances);
        ChallengeDetailMenu.Instance.AmountImage.transform.parent.GetComponentInChildren<TMP_Text>().text = "Level " + (Level - 1);
        ChallengeDetailMenu.Instance.AmountImage.fillAmount = (float)((float)Distance / (float)LevelDistances[Level - 1]);
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
