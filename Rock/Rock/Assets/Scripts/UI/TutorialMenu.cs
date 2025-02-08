using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : MonoBehaviour
{
    public GameObject SpaceButtonText;
    public Button TutorialDoneButton;

    public static TutorialMenu Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
#if UNITY_ANDROID
        SpaceButtonText.SetActive(false);
#endif

        TutorialDoneButton.onClick.AddListener(() => SaveManager.Instance.TutorialDone = true);
        TutorialDoneButton.onClick.AddListener(() => SaveManager.Instance.Save());
    }


}
