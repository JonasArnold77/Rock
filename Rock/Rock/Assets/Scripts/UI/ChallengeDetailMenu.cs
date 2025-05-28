using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RestartButton.onClick.AddListener(() => Respawn());
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Instantiate(PrefabManager.Instance.MedalRevealUIEffect,SilverMedalGO.transform.position, Quaternion.identity);
        Instantiate(PrefabManager.Instance.MedalRevealUIEffect, GoldMedalGO.transform.position, Quaternion.identity);
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        //string currentSceneName = SceneManager.GetActiveScene().name;
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("Gravity Run");
    }
}
