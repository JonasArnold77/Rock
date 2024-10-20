using UnityEngine;

public class WobbleEffect : MonoBehaviour
{
    // Parameter zur Anpassung des Wobble-Effekts
    public float wobbleAmount = 0.1f; // Die maximale Skalierung des Wobble-Effekts
    public float wobbleSpeed = 5f;    // Die Geschwindigkeit des Wobble-Effekts

    private Vector3 initialScale;     // Die urspr�ngliche Skalierung des Sprites
    private Rigidbody2D rb;           // Rigidbody2D f�r die Velocity

    void Start()
    {
        // Speichere die anf�ngliche Skalierung des Sprites
        initialScale = transform.localScale;

        // Zugriff auf das Rigidbody2D-Component
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Berechne die St�rke des Wobble-Effekts basierend auf der Geschwindigkeit
        float wobbleFactor = rb.velocity.magnitude * wobbleAmount;

        // Verwende eine Sinusfunktion f�r eine weiche Wobble-Bewegung
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleFactor;

        // Setze die neue Skalierung des Sprites, basierend auf dem Wobble-Wert
        transform.localScale = new Vector3(initialScale.x + wobble, initialScale.y - wobble, initialScale.z);
    }
}