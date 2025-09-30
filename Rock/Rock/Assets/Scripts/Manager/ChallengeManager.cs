using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        FindObjectOfType<PlayerMovement>().speed = 6;
        SaveManager.Instance.HardcoreModeOn = false;
        Physics2D.gravity = new Vector2(Physics2D.gravity.x, -9.91f);
    }

    public void ActivateNormalEffect()
    {
        FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Rigidbody2D>().sharedMaterial = null;
        FindObjectOfType<PlayerMovement>().speed = 6;
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
        FindObjectOfType<PlayerMovement>().speed = 6;
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
        FindObjectOfType<PlayerMovement>().speed = 8;
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
        FindObjectOfType<PlayerMovement>().horizontalSpeed = 8f;
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