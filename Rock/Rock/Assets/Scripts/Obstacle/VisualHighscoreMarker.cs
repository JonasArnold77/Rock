using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualHighscoreMarker : MonoBehaviour
{
    public TMP_Text HighscoreText;

    private void Start()
    {
        HighscoreText = GetComponentInChildren<TMP_Text>();
    }
}
