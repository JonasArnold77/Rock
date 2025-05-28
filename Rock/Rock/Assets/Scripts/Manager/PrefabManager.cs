using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    public GameObject GroundDashEffect;
    public GameObject JumpDashEffect;
    public GameObject DieEffect;
    public GameObject GemExplodeEffect;
    public GameObject ChallengeButton;
    public GameObject VisualHighscoreMarker;

    public GameObject MedalRevealUIEffect;

    public PhysicsMaterial2D BouncyMaterial;

    public GameObject XpText;

    private void Awake()
    {
        Instance = this;
    }
}
