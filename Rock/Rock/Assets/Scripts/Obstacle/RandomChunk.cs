using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomDictionary
{
    public ChallengeType challengeType;
    public List<GameObject> value;
}

[Serializable]
public class ObjectsByChallengeType
{
    public ChallengeType challengeType;
    public List<GameObject> Objects;
}

[Serializable]
public class ObjectsByHeightType
{
    public HeightType heightType;
    public List<GameObject> Objects;
}

public class RandomChunk : MonoBehaviour
{
    public List<ObjectsByChallengeType> ObjectByChallengeType = new List<ObjectsByChallengeType>();
    public List<ObjectsByChallengeType> ObjectByChallengeTypeDisable = new List<ObjectsByChallengeType>();

    public List<ObjectsByHeightType> ActivateObjectsByHeightTyoe = new List<ObjectsByHeightType>();
    public List<ObjectsByHeightType> DeactivateObjectsByHeightTyoe = new List<ObjectsByHeightType>();

    public bool InitIsDone;

    private void Start()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        foreach (var o in ActivateObjectsByHeightTyoe)
        {
            if (o.heightType == GetComponent<Obstacle>().FinalEndType)
            {
                o.Objects.ForEach(o => o.SetActive(true));
            }
            else
            {
                o.Objects.ForEach(o => o.SetActive(false));
            }
        }

        foreach (var o in DeactivateObjectsByHeightTyoe)
        {
            if (o.heightType == GetComponent<Obstacle>().FinalEndType)
            {
                o.Objects.ForEach(o => o.SetActive(false));
            }
            else
            {
                o.Objects.ForEach(o => o.SetActive(true));
            }
        }

        foreach (var o in ObjectByChallengeType)
        {
            if (o.challengeType == ChallengeManager.Instance.actualChallengeButton.ActualChallengeType)
            {
                o.Objects.ForEach(o => o.SetActive(true));
            }
            else
            {
                o.Objects.ForEach(o => o.SetActive(false));
            }
        }

        foreach (var o in ObjectByChallengeTypeDisable)
        {
            if (o.challengeType == ChallengeManager.Instance.actualChallengeButton.ActualChallengeType)
            {
                o.Objects.ForEach(o => o.SetActive(false));
            }
            else
            {
                o.Objects.ForEach(o => o.SetActive(true));
            }
        }
        InitIsDone = true;
    }
}