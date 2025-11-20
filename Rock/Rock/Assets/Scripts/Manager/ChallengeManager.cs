using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;

    public ChallengeTypeDatabase ChallengeTypeDB;

    public ChallengeButton actualChallengeButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitText());
        StartCoroutine(InitHide());
    }

    private IEnumerator InitText()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if(actualChallengeButton.buttonText != "")
        {
            JumpButton.Instance.JumpButtonGO.GetComponentInChildren<Text>().text = actualChallengeButton.buttonText;
        }
    }

    public IEnumerator InitHide()
    {
        yield return new WaitUntil(() => FindObjectOfType<PlayerMovement>().GameIsStarted);
        JumpButton.Instance.JumpButtonGO.SetActive(false);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }

    public void ActivateBouncyBallEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = PrefabManager.Instance.BouncyMaterial;
        PrefabManager.Instance.BouncyMaterial.bounciness = 0.53f;
        FindObjectOfType<PlayerMovement>().jumpForce = 11f;

        FindObjectOfType<PlayerMovement>().raycastDistance = 0.2f;

        FindObjectOfType<PlayerMovement>().speed = 6.25f;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -10.91f);
        SaveManager.Instance.HardcoreModeOn = false;
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);

    }

    public void ActivateNormalEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 6.25f;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -12f);

        FindObjectOfType<PlayerMovement>().jumpForce = 12.5f;

        SaveManager.Instance.HardcoreModeOn = false;
        
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }

    public void ActivateGravityChangeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
        FindObjectOfType<PlayerMovement>().speed = 6.5f;
        SaveManager.Instance.HardcoreModeOn = false;
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateStrongGravityChangeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
        FindObjectOfType<PlayerMovement>().speed = 7;
        FindObjectOfType<PlayerMovement>().StrongGravityYVelocity = 15;

        SaveManager.Instance.HardcoreModeOn = false;
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateHardcoreModeEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = true;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateHighspeedEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 8;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }

    public void ActivateMoveWIthBallEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateMoveCameraEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateFlappyEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 5.5f;
        FindObjectOfType<PlayerMovement>().FlappyJumpForce = 5f;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -13.91f);
        FindObjectOfType<PlayerMovement>().raycastDistance = 0.6f;
        SaveManager.Instance.HardcoreModeOn = false;
        JumpButton.Instance.JumpButtonGO.SetActive(true);
        JumpButton.Instance.FollowScaleGO.SetActive(false);
    }
    public void ActivateFollowEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().horizontalSpeed = 9f;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, 0);
        JumpButton.Instance.JumpButtonGO.SetActive(false);
        JumpButton.Instance.FollowScaleGO.SetActive(true);
    }
    public void ActivateClickingEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        SaveManager.Instance.HardcoreModeOn = false;
        JumpButton.Instance.JumpButtonGO.SetActive(false);
        JumpButton.Instance.FollowScaleGO.SetActive(false);

        FindObjectOfType<PlayerMovement>().speed = 6.25f;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -12f);

        FindObjectOfType<PlayerMovement>().jumpForce = 12.5f;
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

    public string CreateWholeSaveString(string dimensionName, string selectedChallenge, int[] completedChallenges)
    {
        if (completedChallenges.Length != 4)
            throw new ArgumentException("completedChallenges muss genau 4 Einträge enthalten.");

        return $"{dimensionName}|{selectedChallenge}|{string.Join(",", completedChallenges)}";
    }

    public string UpdateChallengeName(string saveString, string newChallengeName)
    {
        string[] parts = saveString.Split('|');

        if (parts.Length != 3)
            throw new ArgumentException("Ungültiger Save-String.");

        return $"{parts[0]}|{newChallengeName}|{parts[2]}";
    }

    public string UpdateCompletedChallenges(string saveString, int[] newCompleted)
    {
        if (newCompleted.Length != 4)
            throw new ArgumentException("newCompleted muss genau 4 Einträge enthalten.");

        string[] parts = saveString.Split('|');

        if (parts.Length != 3)
            throw new ArgumentException("Ungültiger Save-String.");

        return $"{parts[0]}|{parts[1]}|{string.Join(",", newCompleted)}";
    }

    public object GetSaveParameter(string saveString, string parameterName)
    {
        string[] parts = saveString.Split('|');

        if (parts.Length != 3)
            throw new ArgumentException("Ungültiger Save-String.");

        switch (parameterName.ToLower())
        {
            case "dimension":
                return parts[0];
            case "challenge":
                return parts[1];
            case "completed":
                return parts[2].Split(',').Select(int.Parse).ToArray(); // int[]
            default:
                throw new ArgumentException($"Unbekannter Parametername: {parameterName}");
        }
    }

    public string FindSaveByChallengeName(List<string> saveList, string challengeName)
    {
        return saveList.FirstOrDefault(s =>
        {
            string[] parts = s.Split('|');
            return parts.Length >= 2 && parts[0] == challengeName;
        });
    }
}