using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float rotationSpeed = 200f;
    public Transform StartPos;
    public Transform EndPos;

    public bool xAxisFixed;
    public bool yAxisFixed;

    public bool Respawn;
    public bool RandomSpeed;
    public bool isStatic;

    public float speed = 1.0f; // Geschwindigkeit der Bewegung

    private bool movingToB = true;

    public bool IsMoving = true;

    public SpriteRenderer spriteRenderer;  // Referenz zum SpriteRenderer
    public float blinkInterval = 0.5f;     // Intervall in Sekunden zwischen den Farbwechseln

    public Color grayColor;
    public Color whiteColor;
    private bool isGray = true;

    public Transform LastParent;
    public bool Started;

    public bool StartFromSetPosition;
    public bool Teleport;
    public int StartOffset = 4;
    public Transform Player;

    private void Start()
    {
        StartCoroutine(WaitForRandomChunk());
    }

    public IEnumerator WaitForRandomChunk()
    {
        if (LastParent.GetComponent<RandomChunk>())
        {
            yield return new WaitUntil(() => LastParent.GetComponent<RandomChunk>().InitIsDone);
        }
        
        transform.parent = null;

        Player = FindObjectOfType<PlayerMovement>().transform;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!StartFromSetPosition && StartPos != null)
        {
            transform.position = StartPos.position;
        }

        if (RandomSpeed)
        {
            speed = Random.Range(11, 16);
        }
    }

    private void Update()
    {
        if (LastParent.GetComponent<RandomChunk>())
        {
            if (!LastParent.GetComponent<RandomChunk>().InitIsDone)
            {
                return;
            }
        }
        
        if (EndPos == null && Vector2.Distance(transform.position, Player.position) > 13)
        {
            return;
        }


        if (Vector2.Distance(Player.position, transform.position) > 50 && Player.position.x > transform.position.x)
        {
            Destroy(gameObject);
        }

        //if (EndPos != null && StartPos != null && Vector2.Distance(EndPos.position, Player.position) > 13 && Vector2.Distance(StartPos.position, Player.position) > 13)
        //{
        //    return;
        //}

        if (LastParent.position.x - FindObjectOfType<PlayerMovement>().gameObject.transform.position.x >= 100)
        {
            return;
        }
        else if(!Started && LastParent.position.x - FindObjectOfType<PlayerMovement>().gameObject.transform.position.x <= 100)
        {
            IsMoving = true;
            Started = true;
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);

        if (isStatic) 
        {
            return;
        }

       
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

        if((Vector2)transform.position == flatEndPos)
        {
            movingToB = false;
            targetPosition = flatStartPos;
        }
        else if((Vector2)transform.position == flatStartPos)
        {
            movingToB = true;
            targetPosition = flatEndPos;
        }

        if (IsMoving)
        {
            // Sprite zur Zielposition bewegen
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            return; 
        }

        // Pr�fen, ob das Sprite die Zielposition erreicht hat
        if ((Vector2)transform.position == targetPosition)
        {
            if (!Respawn)
            {
                // Richtung umkehren
                movingToB = !movingToB;
            }
            else
            {
                //if (!Teleport)
                //{
                   StartCoroutine(InitRespawn());
                //}
                //else
                //{
                //    transform.position = StartPos.position;
                //}
                
            }
        }   
    }

    public IEnumerator InitRespawn()
    {
        StartCoroutine(BlinkSprite());
        IsMoving = false;
        yield return StartCoroutine(CheckIfAnimationFinished(transform, EndPos.position, EndPos.position - new Vector3(0, StartOffset, 0), 2f));
        yield return StartCoroutine(CheckIfAnimationFinished(transform, StartPos.position - new Vector3(0, StartOffset, 0), StartPos.position, 2f));
        IsMoving = true;
        spriteRenderer.color = Color.white;
    }

    private IEnumerator CheckIfAnimationFinished(Transform objectTransform, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        spriteRenderer.color = Color.grey;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpoliert zwischen Start- und Zielposition
            objectTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            // Warten bis zum n�chsten Frame
            yield return null;
        }

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

            // Warte f�r das angegebene Intervall
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
