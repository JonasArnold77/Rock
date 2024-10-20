using System.Collections;
using System.Collections.Generic;
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
    public float spawnInterval = 1f;

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
        if (objectPrefab != null && PlayerTransform != null)
        {
            Vector3 spawnPosition;

            // Wenn dies das erste Objekt ist, platziere es basierend auf dem Spieler
            if (lastSpawnedObject == null)
            {
                spawnPosition = PlayerTransform.position + new Vector3(spawnDistanceAhead, 0, 0);
            }
            else
            {
                // Berechne die Position direkt rechts vom zuletzt generierten Objekt
                float lastObjectWidth = lastSpawnedObject.GetComponent<SpriteRenderer>().bounds.size.x;
                spawnPosition = lastSpawnedObject.transform.position + new Vector3(lastObjectWidth + 6, 0, 0);
            }

            objectPrefab = LevelChunkManager.Instance.Chunks[Random.Range(0, LevelChunkManager.Instance.Chunks.Count)];

            spawnPosition = new Vector3(spawnPosition.x, objectPrefab.GetComponent<Obstacle>().height, spawnPosition.z);
            

            // Erstelle das neue Objekt an der berechneten Position
            GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

            // Setze eine zufällige Skalierung für das neue Objekt
            float randomScale = Random.Range(minScale, maxScale);
            newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, newObject.transform.localScale.y, 1f);

            // Aktualisiere das zuletzt generierte Objekt
            lastSpawnedObject = newObject;

            // Starte das nächste Objekt-Spawning nach dem festgelegten Intervall
            Invoke("SpawnObject", spawnInterval);
        }
        else
        {
            Debug.LogWarning("ObjectPrefab oder PlayerTransform ist nicht zugewiesen.");
        }
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
