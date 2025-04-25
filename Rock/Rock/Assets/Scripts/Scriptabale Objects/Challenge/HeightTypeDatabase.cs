using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Obstacle/HeightTypeDB")]
public class HeightTypeDatabase : ScriptableObject
{
    public HeightType Bottom;
    public HeightType StairUp;
    public HeightType StairDown;
    public HeightType Middle;
    public HeightType Top;

    public List<HeightType> AllTypes = new List<HeightType>();
}
