using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAll : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(FindObjectOfType<PlayerMovement>().transform.position.x, FindObjectOfType<PlayerMovement>().transform.position.y);
    }
}
