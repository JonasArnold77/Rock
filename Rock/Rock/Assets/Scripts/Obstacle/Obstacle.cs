using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float height;
    public EObstacleType startType;
    public EObstacleType endType;

    public int DifficultyLevel;

    public Transform PlayerTransform;

    public Collider2D AreaCollider;

    public EChunkType ChunkType;

    public string Title;

    public int RuntimeID;

    private void Start()
    {
        PlayerTransform = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (!IsPlayerInsideCollider() && Vector2.Distance(PlayerTransform.position,transform.position)>200 && PlayerTransform.position.x > transform.position.x)
        {
             Destroy(gameObject);
        }
    }

    // Funktion, die überprüft, ob sich ein Objekt innerhalb des Ziel-Colliders befindet
    public bool IsPlayerInsideCollider()
    {
        if (AreaCollider == null)
        {
            Debug.LogError("Kein Ziel-Collider zugewiesen!");
            return false;
        }

        // Berechne die Position und die Größe der OverlapBox basierend auf dem Ziel-Collider
        Vector2 boxCenter = AreaCollider.bounds.center;
        Vector2 boxSize = (Vector2)AreaCollider.bounds.size * Vector2.one ;

        // Finde alle Collider in der definierten Box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0);

        // Durchlaufe alle gefundenen Collider und prüfe, ob sich eines davon innerhalb des Ziel-Colliders befindet
        foreach (Collider2D col in colliders)
        {
            if (col != AreaCollider && col.tag == "Player") // Ignoriere den Ziel-Collider selbst
            {
                if (startType != EObstacleType.StairDown && startType != EObstacleType.StairUp && Title != LevelChunkManager.Instance.StartChunk.name && !LevelChunkManager.Instance.BeginningChunks.Select(s => s.name).ToList().Contains(Title))
                {
                    SaveManager.Instance.LastChunk = Title;
                    SaveManager.Instance.LastChunkType = ChunkType.ToString();
                }
                
                return true; // Ein anderes Objekt befindet sich innerhalb
            }
        }

        return false; // Kein Objekt gefunden
    }
}
