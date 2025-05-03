using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Rigidbody2D rb;
    public SpriteRenderer targetSprite;
    public float distanceBetweenGhosts = 0.5f;     // Abstand in Welt-Einheiten
    public float fadeDuration = 0.5f;
    public Vector3 offset = Vector3.zero;

    [Header("Appearance")]
    public Color ghostColor = new Color(1, 1, 1, 0.8f);
    public Vector3 ghostScale = Vector3.one;

    private Vector3 lastSpawnPosition;

    private void Start()
    {
        lastSpawnPosition = transform.position;
        SpawnGhost(); // ersten sofort spawnen
    }

    private void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastSpawnPosition);
        if (distanceMoved >= distanceBetweenGhosts)
        {
            SpawnGhost();
            lastSpawnPosition = transform.position;
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = new GameObject("GhostSprite");
        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = targetSprite.sprite;
        sr.flipX = targetSprite.flipX;
        sr.color = ghostColor;
        sr.sortingLayerID = targetSprite.sortingLayerID;
        sr.sortingOrder = targetSprite.sortingOrder - 1;

        ghost.transform.position = transform.position + offset;
        ghost.transform.localScale = ghostScale;

        // Rotation anhand Velocity
        if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            ghost.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        StartCoroutine(FadeAndDestroy(sr, fadeDuration));
    }

    IEnumerator FadeAndDestroy(SpriteRenderer sr, float duration)
    {
        float time = 0f;
        Color originalColor = sr.color;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, time / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(sr.gameObject);
    }
}
