using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform PlayerTransform;
    public static LevelManager Instance;
    public GameObject LevelBorderGameObject;

    public GameObject objectPrefab;        // Das GameObject, das generiert werden soll
    public float spawnDistanceAhead = 15f; // Entfernung, in der die Objekte vor dem Spieler generiert werden
    public float minScale = 0.5f;          // Minimale Skalierung des Objekts
    public float maxScale = 2f;            // Maximale Skalierung des Objekts
    public float spawnInterval = 0f;

    private GameObject lastSpawnedObject;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnObject();
    }

    private void Update()
    {
        GenerateNewLevelPart();
    }

    public void GenerateNewLevelPart()
    {
        if(PlayerTransform.position.x >= LevelBorderGameObject.transform.position.x)
        {
            CreateCloneToTheRight();
        }
    }

    void SpawnObject()
    {
        if (objectPrefab == null || PlayerTransform == null)
        {
            Debug.LogWarning("ObjectPrefab or PlayerTransform is not assigned.");
            return;
        }

        Vector3 spawnPosition;
        if (lastSpawnedObject != null)
        {
            var obstacle = lastSpawnedObject.GetComponent<Obstacle>();
            if (obstacle == null)
            {
                Debug.LogWarning("Last spawned object does not have an Obstacle component.");
                return;
            }

            var chunkType = ControlPanel.Instance.GetNextLevelChunk(obstacle.actualType);
            var potentialChunks = LevelChunkManager.Instance.Chunks
                .Where(c => c.GetComponent<Obstacle>().actualType == chunkType)
                .ToList();

            if (potentialChunks.Count > 0)
            {
                objectPrefab = potentialChunks[Random.Range(0, potentialChunks.Count)];
            }
            else
            {
                Debug.LogWarning("No chunks available for the specified chunk type.");
                return;
            }
        }
        else
        {
            objectPrefab = LevelChunkManager.Instance.Chunks[Random.Range(0, LevelChunkManager.Instance.Chunks.Count)];
        }

        // Berechne die Spawn-Position
        if (lastSpawnedObject == null)
        {
            spawnPosition = PlayerTransform.position + new Vector3(spawnDistanceAhead, 0, 0);
        }
        else
        {
            float lastObjectWidth = lastSpawnedObject.GetComponent<SpriteRenderer>().bounds.size.x;
            float objectWidth = objectPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            float offset = 0.0f; // optionaler Abstand zwischen den Objekten

            // Berechne die Position direkt neben dem letzten Objekt
            spawnPosition = lastSpawnedObject.transform.position + new Vector3(lastObjectWidth / 2 + objectWidth / 2 + offset, 0, 0);
        }

        spawnPosition = new Vector3(spawnPosition.x, objectPrefab.GetComponent<Obstacle>().height, spawnPosition.z);

        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        float randomScale = Random.Range(minScale, maxScale);
        newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, newObject.transform.localScale.y, 1f);

        lastSpawnedObject = newObject;

        Invoke("SpawnObject", spawnInterval);
    }

    void CreateCloneToTheRight()
    {
        if (LevelBorderGameObject != null)
        {
            // Erstelle eine Kopie des Sprite-GameObjects
            GameObject clone = Instantiate(LevelBorderGameObject);

            // Berechne die Breite des Sprites
            float spriteWidth = LevelBorderGameObject.GetComponent<SpriteRenderer>().bounds.size.x;

            // Setze die Position der Kopie rechts hinter das ursprüngliche Sprite-GameObject
            clone.transform.position = LevelBorderGameObject.transform.position + new Vector3(spriteWidth, 0, 0);
            LevelBorderGameObject = clone;
        }
        else
        {
            Debug.LogWarning("spriteToClone ist nicht zugewiesen.");
        }
    }
}
