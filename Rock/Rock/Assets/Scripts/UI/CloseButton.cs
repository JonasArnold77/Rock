using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    public GameObject Window;
    public List<GameObject> WindowList = new List<GameObject>();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => CloseWindow());
    }

    private void CloseWindow()
    {
        if (WindowList.Count > 0)
        {
            WindowList.ForEach(w => w.SetActive(false));
            return;
        }

        Window.SetActive(false);
    }
}
