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

    public static AdManager Instance;
    private void Awake()
    {
        Instance = this;   
    }

    public IEnumerator StartInterstitial()
    {
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
}
