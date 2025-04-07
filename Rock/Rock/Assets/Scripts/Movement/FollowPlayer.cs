using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;
    public float distance;

    public float radius = 2f; // Maximaler Abstand in local space
    public float moveSpeed = 1.5f; // Bewegungsgeschwindigkeit
    public float arriveThreshold = 0.05f; // Abstand, bei dem Ziel als erreicht gilt

    private Vector3 targetLocalPos;

    void Update()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, PlayerTransform.position.y + 3, transform.position.z);
        }
        else if(ChallengeManager.Instance.actualChallengeButton.title != "MoveCamera")
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, transform.position.y, transform.position.z);
        }
    }

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title != "MoveCamera")
        {
            return;
        }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.localPosition, targetLocalPos) < arriveThreshold)
        {
            ChooseNewTarget();
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
