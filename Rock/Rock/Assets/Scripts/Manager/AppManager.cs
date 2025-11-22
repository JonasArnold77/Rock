using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    private static bool hasStarted = false;

    void Start()
    {
        if (!hasStarted)
        {
            Instance = this;
            hasStarted = true;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(OnAppStart());
        }
        else
        {
            Destroy(gameObject); // Doppeltes AppManager-Objekt verhindern
        }
    }

    private IEnumerator OnAppStart()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        //ChallengeDetailMenu.Instance.HidePanelImage.color = new Color(ChallengeDetailMenu.Instance.HidePanelImage.color.r, ChallengeDetailMenu.Instance.HidePanelImage.color.g, ChallengeDetailMenu.Instance.HidePanelImage.color.b,1);
        FindObjectOfType<PlayerMovement>().DirectReset();
    }
}
