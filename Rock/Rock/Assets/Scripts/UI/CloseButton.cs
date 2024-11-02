using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    public GameObject Window;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => CloseWindow());
    }

    private void CloseWindow()
    {
        Window.SetActive(false);
    }
}
