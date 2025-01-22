using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform PlayerTransform;
    public static LevelManager Instance;
    public GameObject LevelBorderGameObject;

    public bool FirstChunkSetted;
    public bool LastFirstObjectSetted;
    public bool StairSetted;

    public GameObject objectPrefab;        // Das GameObject, das generiert werden soll
    public float spawnDistanceAhead = 15f; // Entfernung, in der die Objekte vor dem Spieler generiert werden
    public float minScale = 0.5f;          // Minimale Skalierung des Objekts
    public float maxScale = 2f;            // Maximale Skalierung des Objekts
    public float spawnInterval = 0f;

    public float spawnDistanceThreshold;

    public int countOfArea;
    public EChunkType actualChunkType;

    public int testLevelCount;

    private GameObject lastSpawnedObject;

    private GameObject lastObject;

    public bool GameIsInitialized;

    public string FirstObjectString;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => GameIsInitialized);
        SpawnObject();
    }

    private void Update()
    {
        if (!GameIsInitialized)
        {
            return;
        }

        GenerateNewLevelPart();

        if (lastSpawnedObject != null && PlayerTransform != null)
        {
            // Berechne die Distanz zwischen dem Spieler und dem letzten Objekt
            float distanceToLastObject = Mathf.Abs(PlayerTransform.position.x - lastSpawnedObject.transform.position.x);

            // Wenn die Distanz kleiner als der Schwellenwert ist, spawne das nächste Objekt
            if (distanceToLastObject <= spawnDistanceThreshold)
            {
                SpawnObject();
            }
        }
    }

    public void GenerateNewLevelPart()
    {
        if(PlayerTransform.position.x >= LevelBorderGameObject.transform.position.x-8)
        {
            CreateCloneToTheRight();
        }
    }

    void SpawnObject()
    {
        if (PlayerTransform == null)
        {
            Debug.LogWarning("PlayerTransform is not assigned.");
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

            var chunkType = ControlPanel.Instance.GetNextLevelChunk(obstacle.endType);

            if (!FindObjectOfType<PlayerMovement>().IsDead)
            {
                if (countOfArea <= 0)
                {
                    actualChunkType = GetRandomEnumValueExcluding<EChunkType>(actualChunkType, EChunkType.FloorIsLava);
                    countOfArea = UnityEngine.Random.Range(4, 6);
                }
                else if (chunkType != EObstacleType.StairDown && chunkType != EObstacleType.StairUp)
                {
                    countOfArea--;
                }
            }
            

            SaveManager.Instance.CountOfArea = countOfArea;
            SaveManager.Instance.Save();

            //if(countOfArea == 1)
            //{
            //    SaveManager.Instance.CountOfArea = countOfArea;
            //    SaveManager.Instance.Save();
            //}


            var potentialChunks = LevelChunkManager.Instance.Chunks
                .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                .ToList();
            if (SaveManager.Instance.HardcoreModeOn)
            {
                potentialChunks = LevelChunkManager.Instance.HardcoreChunks
                .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                .ToList();
            }

            if (actualChunkType == EChunkType.FloorIsLava)
            {
                chunkType = EObstacleType.Bottom;
                potentialChunks = LevelChunkManager.Instance.Chunks
                    .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                    .ToList();
                if (SaveManager.Instance.HardcoreModeOn)
                {
                    potentialChunks = LevelChunkManager.Instance.HardcoreChunks
                    .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                    .ToList();
                }
            }

            if (chunkType != EObstacleType.StairDown && chunkType != EObstacleType.StairUp)
            {
                potentialChunks = potentialChunks
                    .Where(c => c.GetComponent<Obstacle>().ChunkType == actualChunkType && lastObject.GetComponent<Obstacle>().RuntimeID != c.GetComponent<Obstacle>().RuntimeID)
                    .ToList();
            }

            if (potentialChunks.Count > 0)
            {
                objectPrefab = potentialChunks[UnityEngine.Random.Range(0, potentialChunks.Count)];
            }
            else
            {
                Debug.LogWarning(actualChunkType + " and  " + chunkType + " " + "No chunks available for the specified chunk type.");
                return;
            }
        }
        else
        {
            objectPrefab = LevelChunkManager.Instance.Chunks[UnityEngine.Random.Range(0, LevelChunkManager.Instance.Chunks.Count)];
            if (SaveManager.Instance.HardcoreModeOn) 
            {
                objectPrefab = LevelChunkManager.Instance.HardcoreChunks[UnityEngine.Random.Range(0, LevelChunkManager.Instance.HardcoreChunks.Count)];
            }
        
        }


        if (testLevelCount > 0 && !SaveManager.Instance.HardcoreModeOn)
        {
            if(testLevelCount == 3)
            {
                testLevelCount--;
                objectPrefab = LevelChunkManager.Instance.BeginningChunks[UnityEngine.Random.Range(0, LevelChunkManager.Instance.BeginningChunks.Count)];
            }
            else
            {
                testLevelCount--;
                objectPrefab = LevelChunkManager.Instance.BeginningChunks[UnityEngine.Random.Range(0, LevelChunkManager.Instance.BeginningChunks.Count)];
            }  
        }

        if(FirstObjectString == "" && SaveManager.Instance.HardcoreModeOn)
        {
            objectPrefab = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().startType != EObstacleType.StairDown && h.GetComponent<Obstacle>().startType != EObstacleType.StairUp).FirstOrDefault()/*.GetComponent<Obstacle>().Title*/;
        }

        var z = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().startType != EObstacleType.StairDown && h.GetComponent<Obstacle>().startType != EObstacleType.StairUp).ToList();

        var x = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).ToList();

        var y = LevelChunkManager.Instance.HardcoreChunks.FirstOrDefault().name;


        if (FirstObjectString != "" && SaveManager.Instance.HardcoreModeOn && ((LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).FirstOrDefault().GetComponent<Obstacle>().startType == EObstacleType.Middle && StairSetted) || LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).FirstOrDefault().GetComponent<Obstacle>().startType != EObstacleType.Middle) && !LastFirstObjectSetted && FirstChunkSetted && SaveManager.Instance.HardcoreModeOn && LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).ToList().Count != 0)
        {
            objectPrefab = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).FirstOrDefault();
            LastFirstObjectSetted = true;
        }

        if (FirstObjectString != "" && SaveManager.Instance.HardcoreModeOn && !LastFirstObjectSetted && FirstChunkSetted && !StairSetted)
        {
            if(LevelChunkManager.Instance.HardcoreChunks.Where(h => h.name == FirstObjectString).FirstOrDefault().GetComponent<Obstacle>().startType == EObstacleType.Middle)
            {
                objectPrefab = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().startType == EObstacleType.StairUp).FirstOrDefault();
            }

            StairSetted = true;
        }


        if (!FirstChunkSetted /*&& !SaveManager.Instance.HardcoreModeOn*/)
        {
            objectPrefab = LevelChunkManager.Instance.StartChunk;
            FirstChunkSetted = true;
        }

        

        if (/*FirstChunkSetted && */LevelChunkManager.Instance.TestChunk != null && LevelChunkManager.Instance.TestMode)
        {
            objectPrefab = LevelChunkManager.Instance.TestChunk;
        }

        if (lastSpawnedObject == null)
        {
            spawnPosition = PlayerTransform.position + new Vector3(spawnDistanceAhead, 0, 0);
        }
        else
        {
            float lastObjectWidth = lastSpawnedObject.GetComponent<SpriteRenderer>().bounds.size.x;
            float objectWidth = objectPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
            float offset = 0.0f;

            spawnPosition = lastSpawnedObject.transform.position + new Vector3(lastObjectWidth / 2 + objectWidth / 2 + offset, 0, 0);
        }

        spawnPosition = new Vector3(spawnPosition.x, objectPrefab.GetComponent<Obstacle>().height, spawnPosition.z);

        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        newObject.GetComponent<Obstacle>().Title = objectPrefab.name;

        lastObject = newObject;


        float randomScale = UnityEngine.Random.Range(minScale, maxScale);
        newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, newObject.transform.localScale.y, 1f);

        lastSpawnedObject = newObject;
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

    public T GetRandomEnumValueExcluding<T>(T excludedValue, T excludedValue2) where T : Enum
    {
        // Alle Werte der Enum laden
        T[] allValues = (T[])Enum.GetValues(typeof(T));

        // Werte filtern, um den ausgeschlossenen Wert zu entfernen
        T[] filteredValues = allValues.Where(value => !value.Equals(excludedValue) && !value.Equals(excludedValue2)).ToArray();

        // Zufälligen Wert aus den gefilterten Werten auswählen
        return filteredValues[UnityEngine.Random.Range(0, filteredValues.Length)];
    }
}
