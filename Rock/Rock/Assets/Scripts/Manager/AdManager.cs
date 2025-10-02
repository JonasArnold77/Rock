using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Services.LevelPlay;
using Unity.VisualScripting;

public class AdManager : MonoBehaviour
{
    private LevelPlayBannerAd bannerAd;
    private bool bannerLoaded = false;

    [SerializeField] private string adUnitId = "7miijpcrt6pknyss";
    [SerializeField] private float displayDuration = 10f;

    public int AmountOfLevelsTillAd;

    public PlayerMovement player;

    public static AdManager Instance;
    private void Awake()
    {
        Instance = this;   
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        yield return new WaitUntil(() => LevelManager.Instance.GameIsInitialized);

        if(SaveManager.Instance.FirstSecondsWithoutAd > 0)
        {
            StartCoroutine(WaitForTimeIsDone());
        }

        if (SaveManager.Instance.FirstSecondsWithoutAd == 0)
        {
            StartCoroutine(WaitForLevelCounterDone());
        }
    }

    public IEnumerator WaitForTimeIsDone()
    {
        if (!player.IsDead)
        {
            if(SaveManager.Instance.FirstSecondsWithoutAd > 0)
            {
                yield return new WaitForSeconds(1);
                SaveManager.Instance.FirstSecondsWithoutAd--;
                StartCoroutine(WaitForTimeIsDone());
            }
            else
            {
                SaveManager.Instance.Save();
            }  
        }
        else
        {
            SaveManager.Instance.Save();
        }
    }

    public IEnumerator WaitForLevelCounterDone()
    {
        yield return new WaitUntil(() => player.IsDead);

        if (SaveManager.Instance.LevelsTillAdClip > 1)
        {
            SaveManager.Instance.LevelsTillAdClip--;
        }
        else
        {
            SaveManager.Instance.LevelsTillAdClip = AmountOfLevelsTillAd;
            StartCoroutine(StartInterstitial());
        }

        SaveManager.Instance.Save();
    }

    public IEnumerator StartInterstitial()
    {
        yield break;
        GetComponent<LevelPlaySample>().interstitialAd.LoadAd();

        yield return new WaitUntil(() => GetComponent<LevelPlaySample>().interstitialAd.IsAdReady());

        GetComponent<LevelPlaySample>().interstitialAd.ShowAd();
    }

    public IEnumerator StartBanner()
    {
        if (GetComponent<LevelPlaySample>().bannerAd == null)
        {
            var config = new LevelPlayBannerAd.Config.Builder()
                .SetSize(LevelPlayAdSize.MEDIUM_RECTANGLE)
                .SetPosition(LevelPlayBannerPosition.TopLeft)
                .SetDisplayOnLoad(false)
                .Build();

            GetComponent<LevelPlaySample>().bannerAd = new LevelPlayBannerAd(adUnitId, config);

            GetComponent<LevelPlaySample>().bannerAd.OnAdLoaded += (info) =>
            {
                Debug.Log("Banner erfolgreich geladen.");
                bannerLoaded = true;
            };

            bannerAd.OnAdLoadFailed += (error) =>
            {
                
            };
        }

        bannerLoaded = false;
        GetComponent<LevelPlaySample>().bannerAd.LoadAd();

        float timeout = 10f;
        float elapsed = 0f;
        while (!bannerLoaded && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!bannerLoaded)
        {
            Debug.LogWarning("Banner wurde nicht rechtzeitig geladen.");
            yield break;
        }

        GetComponent<LevelPlaySample>().bannerAd.ShowAd();
        Debug.Log("Banner wird angezeigt.");

        yield return new WaitForSeconds(displayDuration);

        GetComponent<LevelPlaySample>().bannerAd.HideAd();
        Debug.Log("Banner wurde versteckt.");
    }

    private void OnDisable()
    {
        // Alle MonoBehaviour-Objekte finden
        MonoBehaviour[] allBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var mb in allBehaviours)
        {
            // Prüfen, ob der Script-Name "RewardedPrefab" ist
            if (mb.GetType().Name.Contains("RewardedPrefab") || mb.GetType().Name.Contains("BannerPrefab") || mb.GetType().Name.Contains("InterstitialPrefab"))
            {
                // Prüfen, ob es ein DontDestroyOnLoad Objekt ist
                if (mb.gameObject.scene.name == null || mb.gameObject.scene.name == "")
                {
                    Destroy(mb.gameObject);
                }
            }
        }
    }
}
