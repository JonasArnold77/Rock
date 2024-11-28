using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenButton : MonoBehaviour
{
    public GameObject Window;
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OpenWindow());
    }

    private void OpenWindow()
    {
        Window.SetActive(true);
    }
}
