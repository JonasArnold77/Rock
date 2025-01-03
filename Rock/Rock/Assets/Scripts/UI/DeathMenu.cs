using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public static DeathMenu Instance;

    public Button ShopButton;
    public Button RestartButton;

    public Text ScoreText;
    public Text HighscoreText;

    private void Start()
    {
        gameObject.SetActive(false);
        RestartButton.onClick.AddListener(() => Respawn());
        ShopButton.onClick.AddListener(() => ShopMenu.Instance.gameObject.SetActive(true));
        ShopButton.onClick.AddListener(() => ShopMenu.Instance.SetCurrentSkin());
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
