using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public MMF_Player CollectingFeedback;

    public static FeedbackManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
