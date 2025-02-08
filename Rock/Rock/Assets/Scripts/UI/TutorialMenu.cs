using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject SpaceButtonText;

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
    }


}
