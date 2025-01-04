using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public AudioSource _audioSource;

    private void Start()
    {
        var random = Random.Range(0,2);

        if(random == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerMovement>())
        {
            _audioSource.Play();
            GetComponent<SpriteRenderer>().enabled = false;

            foreach (Transform child in transform)
            {
                if(child != transform)
                {
                    Destroy(child);
                }
            }

            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));

            StartCoroutine(WaitForDestroy());
        }
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitUntil(()=> !_audioSource.isPlaying);
        Destroy(gameObject);
    }
}
