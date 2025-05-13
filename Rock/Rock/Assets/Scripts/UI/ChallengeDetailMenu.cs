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

    private void Respawn()
    {
        //SaveManager.Instance.Save();

        //string currentSceneName = SceneManager.GetActiveScene().name;
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("Gravity Run");
    }
}
