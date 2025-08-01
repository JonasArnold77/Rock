using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;

    public ChallengeTypeDatabase ChallengeTypeDB;

    public ChallengeButton actualChallengeButton;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateBouncyBallEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = PrefabManager.Instance.BouncyMaterial;
        FindObjectOfType<PlayerMovement>().speed = 7;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }

    public void ActivateNormalEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }

    public void ActivateGravityChangeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
        SaveManager.Instance.HardcoreModeOn = false;
    }
    public void ActivateStrongGravityChangeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
        SaveManager.Instance.HardcoreModeOn = false;
    }
    public void ActivateHardcoreModeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = true;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
    public void ActivateHighspeedEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 7;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }

    public void ActivateMoveWIthBallEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
    public void ActivateMoveCameraEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
    public void ActivateFlappyEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 5f;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
        SaveManager.Instance.HardcoreModeOn = false;
    }
    public void ActivateFollowEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().horizontalSpeed = 10f;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
    }
    public void ActivateClickingEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
    public void ActivateDashEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0f);
    }
    public void ActivateUpsideDownEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
    public void ActivateRotatingCameraEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }
}