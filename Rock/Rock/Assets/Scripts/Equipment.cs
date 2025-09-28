using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class Equipment : MonoBehaviour
{
    public static Equipment Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GetComponentsInChildren<SpriteRenderer>().ToList().Count > 0)
        {
            foreach (var s in GetComponentsInChildren<SpriteRenderer>().ToList())
            {
                if(GetComponentsInChildren<SpriteRenderer>().ToList().IndexOf(s) != 0  && GetComponentsInChildren<ParticleSystem>().ToList().Count > 0)
                {
                    s.enabled = false;
                }
                else if(GetComponentsInChildren<SpriteRenderer>().ToList().IndexOf(s) != 1 && GetComponentsInChildren<ParticleSystem>().ToList().Count == 0)
                {

                }
            }
        }

        if(GetComponentsInChildren<ParticleSystem>().ToList().Count > 0)
        {
            GetComponentsInChildren<ParticleSystem>().ToList().ForEach(s => s.Stop());
        }

        StartCoroutine(WaitForStart());
    }

    private IEnumerator WaitForStart()
    {
        yield return new WaitUntil(()=>FindObjectOfType<PlayerMovement>().GameIsStarted);

        if (GetComponentsInChildren<SpriteRenderer>().ToList().Count > 0)
        {
            foreach (var s in GetComponentsInChildren<SpriteRenderer>().ToList())
            {
                s.enabled = true;
            }
        }

        if (GetComponentsInChildren<ParticleSystem>().ToList().Count > 0)
        {
            GetComponentsInChildren<ParticleSystem>().ToList().ForEach(s => s.Play());
        }
    }
}
