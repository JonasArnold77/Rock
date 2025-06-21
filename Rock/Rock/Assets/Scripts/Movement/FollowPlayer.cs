using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;
    public float distance;

    public float radius = 2f; // Maximaler Abstand in local space
    public float moveSpeed = 1.5f; // Bewegungsgeschwindigkeit
    public float arriveThreshold = 0.05f; // Abstand, bei dem Ziel als erreicht gilt

    public float rotationSpeed = 30f; // Grad pro Sekunde

    private Vector3 targetLocalPos;


    void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);
        if (ChallengeManager.Instance.actualChallengeButton.title == "UpsideDown")
        {
            transform.rotation = new Quaternion(0, 0, 180f, 0);
        }
    }

    void Update()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, PlayerTransform.position.y + 3, transform.position.z);
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "RotateCamera")
        {
            transform.position = new Vector3(PlayerTransform.position.x, transform.position.y, transform.position.z);
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title != "MoveCamera" && ChallengeManager.Instance.actualChallengeButton.title != "RotateCamera")
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, transform.position.y, transform.position.z);
        }

    }

    void LateUpdate()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera")
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, targetLocalPos) < arriveThreshold)
            {
                ChooseNewTarget();
            }
        }  
    }

    public void ChooseNewTarget()
    {
        // Zufälliger Punkt in Kreis um (0,0)
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(0.5f, radius); // Min. Abstand optional
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        targetLocalPos = new Vector3(x, y, transform.localPosition.z); // Z bleibt wie gehabt
    }
}
