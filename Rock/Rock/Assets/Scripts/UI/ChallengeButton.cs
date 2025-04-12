using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChallengeButton : MonoBehaviour
{
    public UnityEvent ChallengeFunction;
    public string title;
    public int Level;
    public int Highscore;
    public Button RestartButton;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ChallengeFunction.Invoke());
        GetComponent<Button>().onClick.AddListener(() => SetActualChallenge());
        GetComponent<Button>().onClick.AddListener(() => SaveManager.Instance.Save());
    }

    public void SetActualChallenge()
    {
        ChallengeManager.Instance.actualChallengeButton = this;
        SaveManager.Instance.ActualChallenge = title;
        DeathMenu.Instance.SetUpChallengeButtons();
        //GetComponentsInChildren<Button>().Where(b => !b.Equals(GetComponent<Button>())).FirstOrDefault().gameObject.SetActive(true);
        GetComponentsInChildren<Button>()[1].onClick.AddListener(() => Respawn());
    }

    private void Respawn()
    {
        SaveManager.Instance.Save();

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
