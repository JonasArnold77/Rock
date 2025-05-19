using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingWings : MonoBehaviour
{
    [Header("Flap Settings")]
    public float flapSpeed = 2f;             // Geschwindigkeit des Schlagens
    public float flapAmount = 0.2f;          // Wie stark die Skalierung variiert
    public Vector3 originalScale = Vector3.one; // Ausgangsskalierung

    private float time;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        time += Time.deltaTime * flapSpeed;

        // Sinuswelle für gleichmäßiges "Schlagen"
        float scaleOffset = Mathf.Sin(time) * flapAmount;

        // Neue Skalierung anwenden – z.B. nur auf der Y-Achse
        transform.localScale = new Vector3(
            originalScale.x + scaleOffset,
            originalScale.y ,
            originalScale.z
        );
    }
}
