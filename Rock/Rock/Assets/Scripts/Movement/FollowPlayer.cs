using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;
    public float distance;


    void Update()
    {
        if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, PlayerTransform.position.y + 3, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(PlayerTransform.position.x + distance, transform.position.y, transform.position.z);
        }
    }
}
