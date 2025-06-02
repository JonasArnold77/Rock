using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickingSphere : MonoBehaviour
{
    public Transform PlayerTransform;
    // Start is called before the first frame update
    void Start()
    {
        PlayerTransform = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y);
    }
}
