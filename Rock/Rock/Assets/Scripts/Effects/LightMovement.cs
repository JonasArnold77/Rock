using FunkyCode;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public List<Transform> points; // Liste der Punkte
    public float speed = 5f;    // Bewegungsgeschwindigkeit

    private int currentPointIndex = 0; // Aktueller Zielpunkt
    private bool isMoving = true;      // Bewegung aktiv

    private void Start()
    {
        transform.position = points.FirstOrDefault().position;
    }

    void Update()
    {
        if (points == null || points.Count == 0 || !isMoving)
            return;

        MoveTowardsCurrentPoint();

        if (FindObjectOfType<PlayerMovement>().FireBallEffect.activeSelf)
        {
            //SetColor(Color.red);
            if (SaveManager.Instance.HardcoreModeOn)
            {
                //SetColor(Color.red);
                SetColor(InventoryManager.Instance.HardcoreRed);
            }
            else
            {
                SetColor(InventoryManager.Instance.NormalRed);

            }
        }
        else
        {
            if (SaveManager.Instance.HardcoreModeOn)
            {
                SetColor(InventoryManager.Instance.HardcoreYellow);
            }
            else
            {
                SetColor(InventoryManager.Instance.NormalBlue);
            }
        }
    }

    private void MoveTowardsCurrentPoint()
    {
        // Zielpunkt in Weltkoordinaten
        Vector3 targetPoint = points[currentPointIndex].position;
        Vector3 currentPosition = transform.position;

        // Bewegung in Richtung Zielpunkt
        transform.position = Vector3.MoveTowards(currentPosition, targetPoint, speed * Time.deltaTime);

        // Pr�fen, ob der Zielpunkt erreicht wurde
        if (Vector3.Distance(currentPosition, targetPoint) <= 0.1f)
        {
            // Zum n�chsten Punkt wechseln
            currentPointIndex = (currentPointIndex + 1) % points.Count;
        }
    }

    public void SetColor(Color color)
    {
        GetComponent<LightSprite2D>().color = color;
    }

    public void StartMoving() => isMoving = true;

    public void StopMoving() => isMoving = false;

}
