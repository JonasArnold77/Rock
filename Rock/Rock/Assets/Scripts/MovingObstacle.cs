using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float moveDistance = -3f; // Die Distanz, die das Objekt bewegt wird.
    public float moveSpeed = 2f;    // Die Geschwindigkeit der Bewegung.

    public float timeOffset;

    private Vector3 startPosition;

    public bool RandomSpeed;
    public float RandomSpeedAmountMin;
    public float RandomSpeedAmountMax;

    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(InitStart());

        if (RandomSpeed)
        {
            moveSpeed = Random.Range(RandomSpeedAmountMin, RandomSpeedAmountMax);
        }
    }

    private IEnumerator InitStart()
    {
        yield return new WaitForSeconds(timeOffset);
        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            // Bewege das Objekt nach unten
            yield return MoveToPosition(startPosition + Vector3.down * moveDistance);

            // Bewege das Objekt wieder nach oben
            yield return MoveToPosition(startPosition);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition; // Korrigiere eventuelle Rundungsfehler
    }
}
