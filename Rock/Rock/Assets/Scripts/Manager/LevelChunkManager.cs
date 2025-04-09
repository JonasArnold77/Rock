using System.Collections;
using System.Collections.Generic;
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
        //if (ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode")
        //{
        //    Chunks = BouncyBallChunks;
        //}
        //else if (ChallengeManager.Instance.actualChallengeButton.title == "Gravity")
        //{
        //    Chunks = GravityChunks;
        //}
        //else if (ChallengeManager.Instance.actualChallengeButton.title == "StrongGravity")
        //{
        //    Chunks = StrongGravityChunks;
        //}
        //else if (ChallengeManager.Instance.actualChallengeButton.title == "Highspeed")
        //{
        //    Chunks = Chunks;
        //}
        //else if (ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall")
        //{
        //    Chunks = Chunks;
        //}
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
