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
