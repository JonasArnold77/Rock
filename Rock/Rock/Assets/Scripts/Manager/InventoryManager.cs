using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<GameObject> SkinList = new List<GameObject>();

    public int MoneyAmount;

    public Color JumpingColor;
    public Color MagneticColor;

    public bool IsHardcoreMode;

    private int counter = 0; // Der Zähler
    private float nextThreshold = 5f; // Nächster Schwellenwert auf der X-Achse

    public Text HighscoreTicker;
         
    public int Score;

    public GameObject NormalPostProcessing;
    public GameObject HellPostProcessing;

    public Color hellColorBlue;

    private void Awake()
    {
        Instance = this; 
    }

    private void Start()
    {
        nextThreshold = FindObjectOfType<PlayerMovement>().transform.position.x + nextThreshold;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShopMenu.Instance.gameObject.SetActive(true);
        }

        // Aktuelle Position des Objekts auf der X-Achse
        float currentX = FindObjectOfType<PlayerMovement>().transform.position.x;

        // Prüfen, ob der aktuelle Schwellenwert überschritten wurde
        if (currentX >= nextThreshold)
        {
            counter++; // Zähler um 1 erhöhen
            Debug.Log("Zähler: " + counter);

            Score++;
            

            if (Score > SaveManager.Instance.Highscore)
            {
                SaveManager.Instance.Highscore = Score;
            }

            HighscoreTicker.text = Score.ToString();

            // Nächsten Schwellenwert um 5 erhöhen
            nextThreshold += 5f;
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