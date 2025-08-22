using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChallengeDetailMenu : MonoBehaviour
{
    public Button RestartButton;
    public TMP_Text HeadlineText;
    public TMP_Text DiscriptionText;
    public TMP_Text HighscoreText;

    public GameObject BronceMedalGO;
    public GameObject SilverMedalGO;
    public GameObject GoldMedalGO;

    public Image AmountImage;

    public ChallengeTypeDatabase ChallengeTypeDB;

    public static ChallengeDetailMenu Instance;

    public List<CameraChallengeButton> CameraChallengeButtons;

    public CameraChallengeButton ActualChallengeButton;

    public Image SkinAmountImage;
    public Image Symbol;

    public Color NotDoneColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RestartButton.onClick.AddListener(() => Respawn());
        gameObject.SetActive(false);

        foreach (var c in CameraChallengeButtons)
        {
            c.Outlines.SetActive(false);
            c.GetComponent<Button>().onClick.AddListener(() => ActualCameraChallenge(c));
        }
    }

    public void ActualCameraChallenge(CameraChallengeButton ca)
    {
        var challengeString = ChallengeManager.Instance.FindSaveByChallengeName(SaveManager.Instance.CameraChallengesStrings, ChallengeManager.Instance.actualChallengeButton.title);
        int index = SaveManager.Instance.CameraChallengesStrings.IndexOf(challengeString);

        ca.Outlines.SetActive(true);

        CameraChallengeButtons.Where(c => c.challengeName != ca.challengeName).ToList().ForEach(x => x.Outlines.SetActive(false));

        ActualChallengeButton = ca;

        var list = (int[])ChallengeManager.Instance.GetSaveParameter(challengeString, "completed");

        for(int i = 0; i< list.Length; i++)
        {
            if(list[i] == 1)
            {
                CameraChallengeButtons.Where(c => c.Index == i + 1).FirstOrDefault().ScoreText.text = "Done";
                CameraChallengeButtons.Where(c => c.Index == i + 1).FirstOrDefault().ScorePanel.color = CameraChallengeButtons.Where(c => c.Index == i + 1).FirstOrDefault().GetComponent<Image>().color;
            }
            else
            {
                CameraChallengeButtons.Where(c => c.Index == i + 1).FirstOrDefault().ScoreText.text = "" + ChallengeManager.Instance.actualChallengeButton.CameraChallengeHighscores[i];
                CameraChallengeButtons.Where(c => c.Index == i + 1).FirstOrDefault().ScorePanel.color = NotDoneColor;
            }
        }

        SkinAmountImage.fillAmount = 0f;
        foreach (var c in list.ToList())
        {
            if(c == 1)
            {
                SkinAmountImage.fillAmount = SkinAmountImage.fillAmount + 0.25f;
            }
        }

        SaveManager.Instance.CameraChallengesStrings[index] = ChallengeManager.Instance.UpdateChallengeName(challengeString, ca.challengeName);
        SaveManager.Instance.Save();


    }

    private void OnEnable()
    {
        //Instantiate(PrefabManager.Instance.MedalRevealUIEffect,SilverMedalGO.transform.position, Quaternion.identity);
        //Instantiate(PrefabManager.Instance.MedalRevealUIEffect, GoldMedalGO.transform.position, Quaternion.identity);
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        //string currentSceneName = SceneManager.GetActiveScene().name;
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("Gravity Run");
    }
}
