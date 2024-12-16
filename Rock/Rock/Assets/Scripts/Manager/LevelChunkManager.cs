using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChunkManager : MonoBehaviour
{
    public List<GameObject> Chunks = new List<GameObject>();

    public GameObject StartChunk;
    public GameObject TestChunk;

    public bool TestMode;

    public static LevelChunkManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetRuntimeIDs();
    }

    private void SetRuntimeIDs()
    {
        var count = 1;
        foreach(var c in Chunks)
        {
            c.GetComponent<Obstacle>().RuntimeID = count;
            count++;
        }
    }
}
