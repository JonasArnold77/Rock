using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        MoveToRight();
    }

    private void MoveToRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
