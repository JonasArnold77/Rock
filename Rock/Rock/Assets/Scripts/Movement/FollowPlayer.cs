using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;
    public float distance;

    public float radius = 2f;
    public float moveSpeed = 1.5f;
    public float arriveThreshold = 0.05f;

    private Vector3 targetLocalPos;
    private Coroutine shakeCoroutine;

    private bool isShaking = false;
    private Vector3 shakeOffset = Vector3.zero;

    private Vector3 currentFollowPosition;

    void Update()
    {
        

        if (isShaking)
        {
            // Bleibt an aktueller Position + ShakeOffset
            transform.position = currentFollowPosition + shakeOffset;
            return;
        }

        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        {
            currentFollowPosition = new Vector3(PlayerTransform.position.x + distance, PlayerTransform.position.y + 3, transform.position.z);
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title != "MoveCamera")
        {
            currentFollowPosition = new Vector3(PlayerTransform.position.x + distance, transform.position.y, transform.position.z);
        }

        transform.position = currentFollowPosition;
    }

    void LateUpdate()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title != "MoveCamera" || isShaking)
            return;

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, targetLocalPos) < arriveThreshold)
        {
            ChooseNewTarget();
        }
    }

    public void ChooseNewTarget()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float dist = Random.Range(0.5f, radius);
        float x = Mathf.Cos(angle) * dist;
        float y = Mathf.Sin(angle) * dist;

        targetLocalPos = new Vector3(x, y, transform.localPosition.z);
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;

        float elapsed = 0f;
        currentFollowPosition = transform.position; // speichert aktuelle Position zum Wackeln

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(offsetX, offsetY, 0);

            transform.position = currentFollowPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}
