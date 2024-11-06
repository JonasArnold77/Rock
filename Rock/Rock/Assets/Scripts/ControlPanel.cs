using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public static ControlPanel Instance;

    public int CountTillStair = 3;

    private void Awake()
    {
        Instance = this;
    }

    public EObstacleType GetNextLevelChunk(EObstacleType CurrentObstacleType)
    {
        var chunkTypeList = new List<EObstacleType>();

        if(CountTillStair > 0)
        {
            CountTillStair--;
        }
        else
        {
            CountTillStair = Random.Range(3,5);
        }

        if(CurrentObstacleType == EObstacleType.Bottom)
        {
            chunkTypeList.Add(EObstacleType.Bottom);
            if(CountTillStair == 0)return EObstacleType.StairUp;
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if(CurrentObstacleType == EObstacleType.Middle)
        {
            chunkTypeList.Add(EObstacleType.Middle);
            if (CountTillStair == 0)return EObstacleType.StairDown;
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
