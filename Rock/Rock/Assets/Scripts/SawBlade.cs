using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public Transform StartPos;
    public Transform EndPos;

    public bool xAxisFixed;
    public bool yAxisFixed;

    public bool Respawn;

    public float speed = 1.0f; // Geschwindigkeit der Bewegung

    private bool movingToB = true;

    private bool IsMoving = true;

    public SpriteRenderer spriteRenderer;  // Referenz zum SpriteRenderer
    public float blinkInterval = 0.5f;     // Intervall in Sekunden zwischen den Farbwechseln

    public Color grayColor;
    public Color whiteColor;
    private bool isGray = true;

    private void Start()
    {
        transform.parent = null;
        transform.position = StartPos.position;
    }

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);

       
        Vector2 flatEndPos = new Vector2();
        Vector2 flatStartPos = new Vector2();

        if (xAxisFixed)
        {
            flatEndPos = new Vector2(EndPos.position.x, transform.position.y);
            flatStartPos = new Vector2(StartPos.position.x, transform.position.y);
        }
        else if(yAxisFixed)
        {
            flatEndPos = new Vector2(transform.position.x, EndPos.position.y);
            flatStartPos = new Vector2(transform.position.x, StartPos.position.y);
        }
        

        // Zielposition festlegen basierend darauf, wohin das Sprite gerade bewegt wird
        Vector2 targetPosition = movingToB ? flatEndPos : flatStartPos;

        if (IsMoving)
        {
            // Sprite zur Zielposition bewegen
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        // Prüfen, ob das Sprite die Zielposition erreicht hat
        if ((Vector2)transform.position == targetPosition)
        {
            if (!Respawn)
            {
                // Richtung umkehren
                movingToB = !movingToB;
            }
            else
            {
                IsMoving = false;
                StartCoroutine(CheckIfAnimationFinished(transform, StartPos.position - new Vector3(0, 4, 0), StartPos.position, 2f));
                StartCoroutine(BlinkSprite());
            }
        }   
    }

    private IEnumerator CheckIfAnimationFinished(Transform objectTransform, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        spriteRenderer.color = Color.grey;

        transform.position = StartPos.position - new Vector3(0, 4, 0);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpoliert zwischen Start- und Zielposition
            objectTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            // Warten bis zum nächsten Frame
            yield return null;
        }


        IsMoving = true;
        spriteRenderer.color = Color.white;
        // Setzt die Position am Ende auf die exakte Zielposition
        objectTransform.position = endPosition;
    }

    IEnumerator BlinkSprite()
    {
        while (!IsMoving)
        {
            // Farbe umschalten
            spriteRenderer.color = isGray ? grayColor : whiteColor;
            isGray = !isGray;

            // Warte für das angegebene Intervall
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
