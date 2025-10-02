using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpButton : MonoBehaviour
{
    public GameObject JumpButtonGO;
    public GameObject FollowScaleGO;

    public static JumpButton Instance;

    private void Awake()
    {
        Instance = this;
    }
}
