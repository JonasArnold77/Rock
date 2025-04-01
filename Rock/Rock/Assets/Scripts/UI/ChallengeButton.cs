using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChallengeButton : MonoBehaviour
{
    public UnityEvent ChallengeFunction;
    public string title;
    public int Level;
    public int Highscore;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => ChallengeFunction.Invoke());
        GetComponent<Button>().onClick.AddListener(() => SetActualChallenge());
        GetComponent<Button>().onClick.AddListener(() => SaveManager.Instance.Save());
    }

    private void SetActualChallenge()
    {
        ChallengeManager.Instance.actualChallengeButton = this;
        SaveManager.Instance.ActualChallenge = title;
        DeathMenu.Instance.SetUpChallengeButtons();
    }
}
