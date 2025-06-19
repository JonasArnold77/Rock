using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelChunkManager : MonoBehaviour
{
    public List<GameObject> Chunks = new List<GameObject>();
    public List<GameObject> BeginningChunks = new List<GameObject>();
    public List<GameObject> HardcoreChunks = new List<GameObject>();

    public List<GameObject> BouncyBallChunks = new List<GameObject>();
    public List<GameObject> GravityChunks = new List<GameObject>();
    public List<GameObject> StrongGravityChunks = new List<GameObject>();
    public List<GameObject> HighspeedChunks = new List<GameObject>();

    public List<GameObject> AllChunks = new List<GameObject>();

    public GameObject StartChunk;
    public GameObject TestChunk;

    public bool TestMode;



    public static LevelChunkManager Instance;

    private void Awake()
    {
        Instance = this;
        //HardcoreChunks = Chunks;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if (ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.BouncyMode)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Gravity")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.Gravity)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "StrongGravity")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.StrongGravity)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Highspeed")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.Highspeed)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.FixedCamera)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.StrangeCamera)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Flappy")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.FlappyLight)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Normal")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.Normal)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Follow")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.StrongGravity)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Clicking")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.StrongGravity)).ToList();
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Dash")
        {
            Chunks = AllChunks.Where(a => a.GetComponent<Obstacle>()._ChallengeType.Contains(ChallengeManager.Instance.ChallengeTypeDB.Dash)).ToList();
        }
        SetRuntimeIDs();
    }

    private void SetRuntimeIDs()
    {
        var count = 1;

        //var allChunks = Chunks;
        //allChunks.AddRange(HardcoreChunks);

        foreach(var c in Chunks)
        {
            c.GetComponent<Obstacle>().RuntimeID = count;
            count++;
        }
        foreach (var c in HardcoreChunks)
        {
            c.GetComponent<Obstacle>().RuntimeID = count;
            count++;
        }
    }
}
