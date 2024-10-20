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
    public float spawnInterval = 2f;

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
            // Berechne die Position, an der das Objekt generiert werden soll
            Vector3 spawnPosition = PlayerTransform.position + new Vector3(spawnDistanceAhead, 0, 0);

            // Erstelle eine Kopie des GameObjects an der berechneten Position
            GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

            // Setze eine zufällige Skalierung für das neue Objekt
            float randomScale = Random.Range(minScale, maxScale);
            newObject.transform.localScale = new Vector3(randomScale, randomScale, 1f);

            // Berechne die Breite des neu erstellten Objekts
            float objectWidth = newObject.GetComponent<SpriteRenderer>().bounds.size.x;

            // Starte das nächste Objekt-Spawning nach einem Intervall, das der Breite des Objekts entspricht
            Invoke("SpawnObject", objectWidth);
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
