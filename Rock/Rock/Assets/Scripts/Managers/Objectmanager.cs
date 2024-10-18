using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectmanager : MonoBehaviour
{
    public List<GameObject> GravityGameObjects = new List<GameObject>();

    public static Objectmanager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
