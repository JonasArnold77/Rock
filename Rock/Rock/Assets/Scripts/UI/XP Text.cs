using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPText : MonoBehaviour
{
    public int XpAmount;

    public Text uiText; // Der Text, der animiert wird
    public float duration = 2f; // Gesamtdauer des Effekts
    public Vector2 startAnchoredPosition; // Startposition des Textes (im Canvas)
    public Vector2 endAnchoredPosition; // Endposition des Textes (im Canvas)
    public float startFontSize = 20f; // Anfangsgr��e des Textes
    public float endFontSize = 40f; // Endgr��e des Textes
    private RectTransform rectTransform;

    public Transform PlayerTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        transform.parent = FindObjectOfType<Canvas>().transform;
        transform.localPosition = new Vector2(0, 0);
        uiText = GetComponent<Text>();
        PlayerTransform = FindObjectOfType<PlayerMovement>().transform;
        StartCoroutine(AnimateText());

        startAnchoredPosition = new Vector2(Random.Range(-200,200), Random.Range(-200, 200));
        endAnchoredPosition = new Vector2(startAnchoredPosition.x + Random.Range(-200, 200), startAnchoredPosition.y + Random.Range(100, 200));
    }

    //private void Update()
    //{
    //    transform.position = new Vector2(PlayerTransform.position.x, transform.position.y);
    //}

    IEnumerator AnimateText()
    {
        float elapsedTime = 0f;
        Color initialColor = uiText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Position interpolieren (f�r UI: anchoredPosition)
            rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPosition, endAnchoredPosition, t);

            // Schriftgr��e interpolieren
            uiText.fontSize = Mathf.RoundToInt(Mathf.Lerp(startFontSize, endFontSize, t));

            // Transparenz reduzieren
            uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1f, 0f, t));

            yield return null; // Warten auf den n�chsten Frame
        }

        // Effekt abschlie�en
        uiText.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Vollst�ndig transparent
        Destroy(gameObject);
    }
}
