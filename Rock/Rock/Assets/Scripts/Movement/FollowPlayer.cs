using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;
    public float distance;


    void Update()
    {
        transform.position = new Vector3(PlayerTransform.position.x+ distance, transform.position.y, transform.position.z);
    }
}
