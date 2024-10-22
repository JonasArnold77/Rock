using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance;

    private void Awake()
    {
        Instance = this;
    }

    public EObstacleType GetNextLevelChunk(EObstacleType CurrentObstacleType)
    {
        var chunkTypeList = new List<EObstacleType>();
        if(CurrentObstacleType == EObstacleType.Bottom)
        {
            chunkTypeList.Add(EObstacleType.Bottom);
            chunkTypeList.Add(EObstacleType.StairUp);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if(CurrentObstacleType == EObstacleType.Middle)
        {
            chunkTypeList.Add(EObstacleType.Middle);
            chunkTypeList.Add(EObstacleType.StairDown);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if (CurrentObstacleType == EObstacleType.StairDown)
        {
            //chunkTypeList.Add(EObstacleType.Free);
            chunkTypeList.Add(EObstacleType.Bottom);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if (CurrentObstacleType == EObstacleType.StairUp)
        {
            //chunkTypeList.Add(EObstacleType.Free);
            chunkTypeList.Add(EObstacleType.Middle);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        return EObstacleType.All;
    }

    //public int GetDistanceToNextLevelChunk(EObstacleType CurrentObstacleType)
    //{
    //    if(CurrentObstacleType == EObstacleType.Bottom)
    //    {
    //        return Random.Range();
    //    }
    //}
}
