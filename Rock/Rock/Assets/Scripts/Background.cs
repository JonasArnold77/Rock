using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform PlayerTransform;

    private void Start()
    {
        PlayerTransform = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(PlayerTransform.position, transform.position) > 20 && PlayerTransform.position.x > transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
