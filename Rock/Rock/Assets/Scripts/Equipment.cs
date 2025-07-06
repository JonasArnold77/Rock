using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class Equipment : MonoBehaviour
{
    public static Equipment Instance;

    private void Awake()
    {
        Instance = this;
    }
}
