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
}
