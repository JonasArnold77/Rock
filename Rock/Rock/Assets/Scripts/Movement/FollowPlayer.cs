using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform PlayerTransform;

    void Update()
    {
        transform.position = new Vector3(PlayerTransform.position.x, transform.position.y, transform.position.z);
    }
}
