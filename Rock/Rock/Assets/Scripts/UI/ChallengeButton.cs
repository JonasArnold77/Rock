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

    public string showingTitle;
    public string description;

    public VideoClip Video;

    public List<int> LevelDistances = new List<int>();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ChallengeFunction.Invoke());
        GetComponent<Button>().onClick.AddListener(() => SetActualChallenge());
        GetComponent<Button>().onClick.AddListener(() => SaveManager.Instance.Save());

    }

    private void OnEnable()
    {
        HighscoreText.text = "" + Highscore;
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

        var Level = InventoryManager.Instance.GetLevelFromDistance(Distance,LevelDistances);
        ChallengeDetailMenu.Instance.AmountImage.transform.parent.GetComponentInChildren<TMP_Text>().text = "Level " + (Level - 1);
        ChallengeDetailMenu.Instance.AmountImage.fillAmount = (float)((float)Distance / (float)LevelDistances[Level]);
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
