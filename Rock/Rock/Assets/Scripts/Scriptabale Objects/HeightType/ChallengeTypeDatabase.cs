using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Challenge/ChallengeTypeDB")]
public class ChallengeTypeDatabase : ScriptableObject
{
    public ChallengeType Normal;
    public ChallengeType BouncyMode;
    public ChallengeType Gravity;
    public ChallengeType StrongGravity;
    public ChallengeType Highspeed;
    public ChallengeType FixedCamera;
    public ChallengeType StrangeCamera;
    public ChallengeType FlappyLight;
    public ChallengeType HardCore;
    public ChallengeType Follow;

    public List<ChallengeType> ChallengeTypes = new List<ChallengeType>();
}
