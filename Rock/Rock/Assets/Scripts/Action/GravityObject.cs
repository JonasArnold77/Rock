using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public GameObject obj;

    public bool isActive;

    public Transform PlayerTransform;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
        }
    }

    public void SetAsActive()
    {
        //Instantiate(obj, position: GetClosestPointOnSprite(), Quaternion.identity);
        isActive = true;
    }

    public void SetAsNotActive()
    {
        isActive = false;
    }

    public Vector2 GetClosestPointOnSprite()
    {
        var targetPoint = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y);

        // Überprüfen, ob der SpriteRenderer einen Collider hat
        Collider2D collider = transform.GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("SpriteRenderer hat keinen zugehörigen Collider2D.");
            return Vector2.zero;
        }

        // Nächstgelegenen Punkt auf dem Collider zu dem Zielpunkt ermitteln
        Vector2 closestPoint = collider.ClosestPoint(targetPoint);

        return closestPoint;
    }
}
