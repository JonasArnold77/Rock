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

    public HeightType GetNextLevelChunk(HeightType CurrentObstacleType)
    {
        var chunkTypeList = new List<HeightType>();

        if(CountTillStair > 0 && LevelManager.Instance.InitIsDone)
        {
            CountTillStair--;
        }
        else
        {
            CountTillStair = Random.Range(3,5);
        }

        if(CurrentObstacleType == LevelManager.Instance.HeigtTypeDb.Bottom)
        {
            chunkTypeList.Add(LevelManager.Instance.HeigtTypeDb.Bottom);
            if(CountTillStair == 0)return LevelManager.Instance.HeigtTypeDb.StairUp;
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if(CurrentObstacleType == LevelManager.Instance.HeigtTypeDb.Middle)
        {
            chunkTypeList.Add(LevelManager.Instance.HeigtTypeDb.Middle);
            if (CountTillStair == 0)return LevelManager.Instance.HeigtTypeDb.StairDown;
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if (CurrentObstacleType == LevelManager.Instance.HeigtTypeDb.StairDown)
        {
            //chunkTypeList.Add(EObstacleType.Free);
            chunkTypeList.Add(LevelManager.Instance.HeigtTypeDb.Bottom);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        else if (CurrentObstacleType == LevelManager.Instance.HeigtTypeDb.StairUp)
        {
            //chunkTypeList.Add(EObstacleType.Free);
            chunkTypeList.Add(LevelManager.Instance.HeigtTypeDb.Middle);
            return chunkTypeList[Random.Range(0, chunkTypeList.Count)];
        }
        return LevelManager.Instance.HeigtTypeDb.Top;
    }

    //public int GetDistanceToNextLevelChunk(EObstacleType CurrentObstacleType)
    //{
    //    if(CurrentObstacleType == EObstacleType.Bottom)
    //    {
    //        return Random.Range();
    //    }
    //}
}
