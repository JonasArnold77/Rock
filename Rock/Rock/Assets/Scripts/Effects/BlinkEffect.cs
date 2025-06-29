using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    [Tooltip("Frequenz des Blinkens (Anzahl Wellen pro Sekunde)")]
    public float frequency = 2f;

    [Tooltip("Intensität des Blinkens (0 = kein Effekt, 1 = volle Farbe)")]
    [Range(0f, 1f)]
    public float intensity = 1f;

    [Tooltip("Farbe, zu der geblinkt wird")]
    public Color blinkColor = Color.red;

    [Tooltip("Wie oft soll es pro Blinkphase blinken?")]
    public int blinksPerPhase = 3;

    [Tooltip("Pause nach jeder Blinkphase (Sekunden)")]
    public float pauseBetweenPhases = 1.5f;

    public Graphic uiElement;
    private Color originalColor;

    private float blinkTimer = 0f;
    private float pauseTimer = 0f;
    private bool isBlinking = true;
    private int blinkCount = 0;

    void Start()
    {
        if (uiElement == null)
        {
            Debug.LogError("Kein UI-Element mit Graphic-Komponente gefunden!");
            enabled = false;
            return;
        }

        originalColor = uiElement.color;
    }

    void Update()
    {
        if (isBlinking)
        {
            blinkTimer += Time.deltaTime * frequency * Mathf.PI;

            float sinValue = Mathf.Sin(blinkTimer); // 0 → 1 → 0 (bei π)
            float blend = Mathf.Clamp01(sinValue) * intensity;
            uiElement.color = Color.Lerp(originalColor, blinkColor, blend);

            if (blinkTimer >= Mathf.PI) // ein Blink abgeschlossen
            {
                blinkTimer = 0f;
                blinkCount++;

                if (blinkCount >= blinksPerPhase)
                {
                    isBlinking = false;
                    pauseTimer = 0f;
                    uiElement.color = originalColor;
                    blinkCount = 0;
                }
            }
        }
        else
        {
            pauseTimer += Time.deltaTime;

            if (pauseTimer >= pauseBetweenPhases)
            {
                isBlinking = true;
                blinkTimer = 0f;
                pauseTimer = 0f;
            }
        }
    }
}