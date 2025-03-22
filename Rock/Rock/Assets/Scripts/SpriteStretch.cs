using UnityEngine;

public class SpriteStretch : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform spriteTransform;
    public float stretchFactor = 0.2f; // Wie stark das Sprite gedehnt wird
    public float maxStretch = 1.5f; // Maximale Dehnung
    public float resetSpeed = 5f; // Wie schnell sich das Sprite zurücksetzt
    public float stretchDuration = 1f; // Dauer der Dehnung

    private Vector3 originalScale;
    private float stretchTimer = 0f;

    void Start()
    {
        if (spriteTransform == null) spriteTransform = transform;
        originalScale = spriteTransform.localScale;
    }

    void Update()
    {
        float yVelocity = rb.velocity.y;
        float stretchAmount = Mathf.Clamp(1 + Mathf.Abs(yVelocity) * stretchFactor, 1f, maxStretch);

        if (yVelocity != 0)
        {
            stretchTimer = stretchDuration;
            if (yVelocity > 0) // Aufwärtsbewegung: nach oben dehnen
            {
                spriteTransform.localScale = new Vector3(originalScale.x / stretchAmount, originalScale.y * stretchAmount, originalScale.z);
            }
            else // Abwärtsbewegung: nach unten dehnen
            {
                spriteTransform.localScale = new Vector3(originalScale.x * stretchAmount, originalScale.y / stretchAmount, originalScale.z);
            }
        }
        else if (stretchTimer > 0)
        {
            stretchTimer -= Time.deltaTime;
            if (stretchTimer <= 0)
            {
                spriteTransform.localScale = originalScale;
            }
        }
    }
}
