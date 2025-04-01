using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;

    public ChallengeButton actualChallengeButton;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateBouncyBallEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = PrefabManager.Instance.BouncyMaterial;
    }

    public void ActivateNormalEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
    }

    public void ActivateGravityChangeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
    }
}
