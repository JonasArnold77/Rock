using System.Linq;
using UnityEngine;

public class GravityPull : MonoBehaviour
{
    // Maximaler Wert für die Anziehungskraft
    public float maxPullStrength;

    // Maximaler Abstand, bei dem die volle Anziehungskraft wirkt
    public float maxDistance;

    // Rigidbody2D-Komponente des Objekts
    private Rigidbody2D rb;

    private void Start()
    {
        // Holen der Rigidbody2D-Komponente
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(Objectmanager.Instance.GravityGameObjects.Where(g => g.GetComponent<GravityObject>().isActive).ToList().Count== 0)
        {
            return;
        }

        var targetPoint = Objectmanager.Instance.GravityGameObjects.Where(g => g.GetComponent<GravityObject>().isActive).FirstOrDefault().GetComponent<GravityObject>().GetClosestPointOnSprite();
        // Berechnung der Richtung vom aktuellen Objekt zum Zielpunkt
        Vector2 directionToTarget = (targetPoint - new Vector2(transform.position.x,transform.position.y)).normalized;

        // Berechnung der aktuellen Distanz zum Zielpunkt
        float distanceToTarget = Vector2.Distance(transform.position, targetPoint);

        // Berechnung der Anziehungskraft, die umso stärker wird, je näher das Objekt am Zielpunkt ist
        float pullStrength = Mathf.Clamp(maxPullStrength * (1 - (distanceToTarget / maxDistance)), 0, maxPullStrength);

        // Wenn das Objekt weiter als maxDistance vom Zielpunkt entfernt ist, wird keine Kraft angewendet
        if (distanceToTarget <= maxDistance)
        {
            // Anwenden der Kraft, die das Objekt in Richtung des Zielpunkts zieht
            rb.AddForce(directionToTarget * pullStrength);
        }
    }
}