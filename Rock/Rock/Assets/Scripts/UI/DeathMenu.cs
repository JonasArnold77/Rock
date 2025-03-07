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
    public Button HardcoreModeButton;

    public Text ScoreText;
    public Text HighscoreText;

    public Text test;

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

    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
