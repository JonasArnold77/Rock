using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookInVelocityDirection : MonoBehaviour
{
    public Rigidbody2D targetRigidbody2D;
    public Transform objectToRotate;

    [Tooltip("Minimaler Rotationswinkel (z. B. -45)")]
    public float minAngle = -45f;

    [Tooltip("Maximaler Rotationswinkel (z. B. 45)")]
    public float maxAngle = 45f;

    private void Start()
    {
        objectToRotate = transform;
    }

    void Update()
    {
        Vector2 velocity = targetRigidbody2D.velocity;

        if (velocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            float clampedAngle = Mathf.Clamp(angle, minAngle, maxAngle);
            objectToRotate.rotation = Quaternion.Euler(0, 0, clampedAngle);
        }
    }
}
