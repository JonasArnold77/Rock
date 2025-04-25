using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chunk/ChunkTypeDB")]
public class ChunkTypeDatabase : ScriptableObject
{
    public ChunkType UpAndDown;
    public ChunkType Saw;
    public ChunkType FloorIsLava;

    public List<ChunkType> ChunkTypes = new List<ChunkType>();
}
