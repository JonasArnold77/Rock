
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public static ControlManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        CheckForClickedGravityObject();
    }

    public void CheckForClickedGravityObject()
    {
        // �berpr�ft, ob die linke Maustaste gedr�ckt wurde
        if (Input.GetMouseButtonDown(0))
        {
            // Konvertiert die Mausposition von Bildschirmkoordinaten in Weltkoordinaten
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // F�hrt einen Raycast durch, um zu �berpr�fen, ob das Sprite getroffen wurde
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            // �berpr�ft, ob das Ziel-Sprite getroffen wurde
            if (hit.collider != null && hit.collider.GetComponent<GravityObject>())
            {
                foreach(var g in Objectmanager.Instance.GravityGameObjects)
                {
                    if (g.Equals(hit.collider.gameObject))
                    {
                        g.GetComponent<GravityObject>().SetAsActive();
                    }
                    else
                    {
                        g.GetComponent<GravityObject>().SetAsNotActive();
                    }
                }
            }
        }
    }

}
