using System.Linq;
using UnityEngine;

public class GravityPull : MonoBehaviour
{

    // Stärke der anziehenden Kraft
    public float pullStrength = 5f;

    // Rigidbody2D-Komponente des Objekts
    private Rigidbody2D rb;

    private void Start()
    {
        // Holen der Rigidbody2D-Komponente
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var targetPoint = Objectmanager.Instance.GravityGameObjects.Where(g => g.GetComponent<GravityObject>().isActive).FirstOrDefault().GetComponent<GravityObject>().GetClosestPointOnSprite();
        // Berechnung der Richtung vom aktuellen Objekt zum Zielpunkt
        Vector2 directionToTarget = (targetPoint - new Vector2(transform.position.x,transform.position.y)).normalized;

        // Anwenden der Kraft, die das Objekt in Richtung des Zielpunkts zieht
        rb.AddForce(directionToTarget * pullStrength);
    }
}