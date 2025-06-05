using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    public Transform player;          // Referenz zum Spieler
    public float followSpeed = 5f;    // Geschwindigkeit der Wand
    public float maxDistance = 5f;    // Maximale Entfernung zur Wand

    public bool GameIsStarted;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if(ChallengeManager.Instance.actualChallengeButton.title != "Dash")
        {
            gameObject.SetActive(false);
        }

        yield return new WaitUntil(() => FindObjectOfType<PlayerMovement>().GameIsStarted);
        GameIsStarted = true;
    }

    void Update()
    {
        if (!GameIsStarted)
        {
            return;
        }

        float wallX = transform.position.x;
        float targetX = player.position.x - maxDistance;

        if (wallX < targetX)
        {
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }

        if (!player.GetComponent<PlayerMovement>().IsDead)
        {
            transform.position += Vector3.right * followSpeed * Time.deltaTime;
        }
        
    }
}
