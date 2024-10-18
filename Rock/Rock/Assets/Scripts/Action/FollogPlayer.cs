using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollogPlayer : MonoBehaviour
{
    public Transform PlayerTransform;

    private void Update()
    {
        transform.position = new Vector3(PlayerTransform.transform.position.x, transform.position.y, transform.position.z);
    }
}
