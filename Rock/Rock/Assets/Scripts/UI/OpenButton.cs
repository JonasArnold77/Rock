using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenButton : MonoBehaviour
{
    public GameObject Window;
    public List<GameObject> WindowList = new List<GameObject>();

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OpenWindow());
    }

    private void OpenWindow()
    {
        if(WindowList.Count > 0)
        {
            WindowList.ForEach(w => w.SetActive(true));
            return;
        }

        Window.SetActive(true);
    }
}
