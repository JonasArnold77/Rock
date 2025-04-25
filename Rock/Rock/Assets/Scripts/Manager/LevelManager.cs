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

    public GameObject objectPrefab;
    public GameObject lastObjectPrefab;  // Das GameObject, das generiert werden soll
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

    public List<int> UsedChunks = new List<int>();

    public List<GameObject> HardcoreLevelList = new List<GameObject>();
    public int LevelListCounter;
    public bool StairIsNeeded;

    public EObstacleType NextStairType;

    public HeightTypeDatabase HeigtTypeDb;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
        HardcoreLevelList = ShuffleList(LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().startType != HeigtTypeDb.StairUp && h.GetComponent<Obstacle>().startType != HeigtTypeDb.StairDown).ToList());
    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => GameIsInitialized);

        if (SaveManager.Instance.HardcoreModeOn)
        {
            SpawnHardcoreObject();
        }
        else
        {
            SpawnObject();
        }
    }

    private void Update()
    {
        if (!GameIsInitialized)
        {
            return;
        }
        else
        {

        }

        GenerateNewLevelPart();

        if (lastSpawnedObject != null && PlayerTransform != null)
        {
            // Berechne die Distanz zwischen dem Spieler und dem letzten Objekt
            float distanceToLastObject = Mathf.Abs(PlayerTransform.position.x - lastSpawnedObject.transform.position.x);

            // Wenn die Distanz kleiner als der Schwellenwert ist, spawne das nächste Objekt
            if (distanceToLastObject <= spawnDistanceThreshold)
            {
                if (SaveManager.Instance.HardcoreModeOn)
                {
                    SpawnHardcoreObject();
                }
                else
                {
                    SpawnObject();
                }
            }
        }
    }

    public List<GameObject> ShuffleList(List<GameObject> originalList)
    {
        // Kopieren der Original-Liste in eine neue Liste, damit die Original-Liste unverändert bleibt
        List<GameObject> shuffledList = new List<GameObject>(originalList);
        System.Random rand = new System.Random();

        // Durch die Liste iterieren und jedes Element zufällig vertauschen
        for (int i = shuffledList.Count - 1; i > 0; i--)
        {
            // Zufälligen Index innerhalb des verbleibenden Bereichs auswählen
            int j = rand.Next(0, i + 1);

            // Elemente an den Positionen i und j vertauschen
            GameObject temp = shuffledList[i];
            shuffledList[i] = shuffledList[j];
            shuffledList[j] = temp;
        }

        return shuffledList;
    }

    public void GenerateNewLevelPart()
    {
        if(PlayerTransform.position.x >= LevelBorderGameObject.transform.position.x-8)
        {
            CreateCloneToTheRight();
        }
    }

    void SpawnHardcoreObject()
    {
        if (PlayerTransform == null)
        {
            Debug.LogWarning("PlayerTransform is not assigned.");
            return;
        }

        if (lastSpawnedObject == null)
        {
            lastSpawnedObject = HardcoreLevelList[UnityEngine.Random.Range(0, HardcoreLevelList.Count)];
            objectPrefab = HardcoreLevelList[UnityEngine.Random.Range(0, HardcoreLevelList.Count)];
        }

        Vector3 spawnPosition;
        if (lastSpawnedObject != null)
        {
            if (!StairIsNeeded /*&& LevelListCounter > 0*/ && LevelListCounter < HardcoreLevelList.Count && objectPrefab.GetComponent<Obstacle>().endType == HeigtTypeDb.Middle && HardcoreLevelList[LevelListCounter].GetComponent<Obstacle>().startType == HeigtTypeDb.Bottom)
            {
                objectPrefab = LevelChunkManager.Instance.Chunks.Where(c => c.GetComponent<Obstacle>().startType == HeigtTypeDb.StairDown).FirstOrDefault();
                StairIsNeeded = true;
            }
            else if (!StairIsNeeded /*&& LevelListCounter > 0*/ && LevelListCounter < HardcoreLevelList.Count && objectPrefab.GetComponent<Obstacle>().endType == HeigtTypeDb.Bottom && HardcoreLevelList[LevelListCounter].GetComponent<Obstacle>().startType == HeigtTypeDb.Middle)
            {
                objectPrefab = LevelChunkManager.Instance.Chunks.Where(c => c.GetComponent<Obstacle>().startType == HeigtTypeDb.StairUp).FirstOrDefault();
                StairIsNeeded = true;
            }
            else
            {
                StairIsNeeded = false;
            }

            if (LevelListCounter < HardcoreLevelList.Count && !StairIsNeeded && LastFirstObjectSetted)
            {
                objectPrefab = HardcoreLevelList[LevelListCounter];
                LevelListCounter++;

            }
            else if (!StairIsNeeded && LastFirstObjectSetted)
            {
                HardcoreLevelList = ShuffleList(LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().startType != HeigtTypeDb.StairUp && h.GetComponent<Obstacle>().startType != HeigtTypeDb.StairDown).ToList());
                LevelListCounter++;
            }

            


            if (!LastFirstObjectSetted && StairSetted && FirstObjectString == "")
            {
                objectPrefab = HardcoreLevelList.FirstOrDefault();
                LastFirstObjectSetted = true;
            }
            else if (!LastFirstObjectSetted && StairSetted && LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().name == FirstObjectString).ToList().Count > 0)
            {
                objectPrefab = LevelChunkManager.Instance.HardcoreChunks.Where(h => h.GetComponent<Obstacle>().name == FirstObjectString).FirstOrDefault();
                LastFirstObjectSetted = true;
            }
            else if (!LastFirstObjectSetted && StairSetted)
            {
                objectPrefab = HardcoreLevelList.FirstOrDefault();
                LastFirstObjectSetted = true;
            }

            if (!StairSetted && FirstChunkSetted)
            {
                objectPrefab = LevelChunkManager.Instance.Chunks.Where(c => c.GetComponent<Obstacle>().startType == HeigtTypeDb.StairUp).FirstOrDefault();
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

            lastObjectPrefab = objectPrefab;

            UsedChunks.Add(objectPrefab.GetComponent<Obstacle>().RuntimeID);

            newObject.GetComponent<Obstacle>().Title = objectPrefab.name;

            lastObject = newObject;


            float randomScale = UnityEngine.Random.Range(minScale, maxScale);
            newObject.transform.localScale = new Vector3(newObject.transform.localScale.x, newObject.transform.localScale.y, 1f);

            lastSpawnedObject = newObject;
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
                    actualChunkType = GetRandomEnumValueExcluding<EChunkType>(actualChunkType);
                    countOfArea = UnityEngine.Random.Range(4, 6);
                }
                else if (chunkType != HeigtTypeDb.StairDown && chunkType != HeigtTypeDb.StairUp)
                {
                    countOfArea--;
                }
            }
            

            

            //if(countOfArea == 1)
            //{
            //    SaveManager.Instance.CountOfArea = countOfArea;
            //    SaveManager.Instance.Save();
            //}


            var potentialChunks = LevelChunkManager.Instance.Chunks
                .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                .ToList();

            if (actualChunkType == EChunkType.FloorIsLava)
            {
                chunkType = HeigtTypeDb.Bottom;
                potentialChunks = LevelChunkManager.Instance.Chunks
                    .Where(c => c.GetComponent<Obstacle>().startType == chunkType)
                    .ToList();
            }

            if (chunkType != HeigtTypeDb.StairDown && chunkType != HeigtTypeDb.StairUp)
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

        UsedChunks.Add(objectPrefab.GetComponent<Obstacle>().RuntimeID);

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

    public T GetRandomEnumValueExcluding<T>(T excludedValue) where T : Enum
    {
        // Alle Werte der Enum laden
        T[] allValues = (T[])Enum.GetValues(typeof(T));

        // Werte filtern, um den ausgeschlossenen Wert zu entfernen
        T[] filteredValues = allValues.Where(value => !value.Equals(excludedValue)).ToArray();

        // Zufälligen Wert aus den gefilterten Werten auswählen
        return filteredValues[UnityEngine.Random.Range(0, filteredValues.Length)];
    }

    public T GetRandomEnumValue<T>() where T : Enum
    {
        // Alle Werte der Enum laden
        T[] allValues = (T[])Enum.GetValues(typeof(T));

        // Werte filtern, um den ausgeschlossenen Wert zu entfernen
        T[] filteredValues = allValues.ToArray();

        // Zufälligen Wert aus den gefilterten Werten auswählen
        return filteredValues[UnityEngine.Random.Range(0, filteredValues.Length)];
    }
}
