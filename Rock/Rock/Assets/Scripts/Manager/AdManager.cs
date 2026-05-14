using System.Collections;
using UnityEngine;
using Unity.Services.LevelPlay;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("LevelPlay IDs")]
    [SerializeField] private string appKey = "265b4a13d";
    [SerializeField] private string interstitialAdUnitId = "oh1w9eues3xq1867";

    [Header("Ad Settings")]
    [SerializeField] private float interstitialLoadTimeout = 5f;
    [SerializeField] private bool enableTapToShowInterstitialForTesting = false;
    [SerializeField] private bool enableLevelPlayTestSuite = false;

    public int AmountOfLevelsTillAd = 3;

    [Header("References")]
    public PlayerMovement player;
    public GameObject InputBlockingPanelGO;

    private LevelPlayInterstitialAd interstitialAd;

    private bool isLevelPlayInitialized;
    private bool levelPlayInitFailed;
    private bool hasStartedLevelPlayInit;

    private bool isInterstitialReady;
    private bool isInterstitialLoading;
    private bool isShowingInterstitial;
    private bool showInterstitialWhenLoaded;

    private Coroutine levelAdRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("ANDROID TEST: Doppelter AdManager gefunden. Dieser wird gelöscht.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.parent != null)
        {
            Debug.LogWarning("ANDROID TEST: AdManager war kein Root-GameObject. SetParent(null) wird ausgeführt.");
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.LogWarning("ANDROID TEST: AdManager Start.");

        player = FindObjectOfType<PlayerMovement>();

        if (player == null)
        {
            Debug.LogWarning("ANDROID TEST: PlayerMovement wurde nicht gefunden.");
        }

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(false);
        }

        StartCoroutine(InitRoutine());
    }

    private void Update()
    {
        if (enableTapToShowInterstitialForTesting && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.LogWarning("ANDROID TEST: Test-Tap erkannt. ShowInterstitial wird ausgeführt.");
            ShowInterstitial();
        }
    }

    private IEnumerator InitRoutine()
    {
        Debug.LogWarning("ANDROID TEST: InitRoutine gestartet.");

        yield return new WaitUntil(() =>
            LevelManager.Instance != null &&
            LevelManager.Instance.GameIsInitialized
        );

        Debug.LogWarning("ANDROID TEST: Game ist initialisiert. LevelPlay Init startet.");

        InitLevelPlay();

        yield return new WaitUntil(() => isLevelPlayInitialized || levelPlayInitFailed);

        if (levelPlayInitFailed)
        {
            Debug.LogError("ANDROID TEST: Ad-System wird gestoppt, weil LevelPlay Init fehlgeschlagen ist.");
            yield break;
        }

        Debug.LogWarning("ANDROID TEST: LevelPlay ist initialisiert. Ad-Counter kann starten.");

        if (SaveManager.Instance != null && SaveManager.Instance.FirstSecondsWithoutAd > 0)
        {
            yield return StartCoroutine(WaitForFirstAdFreeTime());
        }

        yield return new WaitUntil(() => player != null && !player.IsDead);

        Debug.LogWarning("ANDROID TEST: Player lebt. LevelAdCounter wird gestartet.");

        StartLevelAdCounter();
    }

    private void InitLevelPlay()
    {
        if (hasStartedLevelPlayInit)
        {
            Debug.LogWarning("ANDROID TEST: LevelPlay Init wurde bereits gestartet.");
            return;
        }

        hasStartedLevelPlayInit = true;

        Debug.LogWarning("ANDROID TEST: InitLevelPlay wird gestartet.");
        Debug.LogWarning("ANDROID TEST: App Key = " + appKey);
        Debug.LogWarning("ANDROID TEST: Interstitial Ad Unit ID = " + interstitialAdUnitId);

        if (enableLevelPlayTestSuite)
        {
            LevelPlay.SetMetaData("is_test_suite", "enable");
        }

        LevelPlay.OnInitSuccess += OnLevelPlayInitSuccess;
        LevelPlay.OnInitFailed += OnLevelPlayInitFailed;

        LevelPlay.Init(appKey);
    }

    private void OnLevelPlayInitSuccess(LevelPlayConfiguration configuration)
    {
        Debug.LogWarning("ANDROID TEST: LevelPlay initialized successfully.");

        isLevelPlayInitialized = true;
        levelPlayInitFailed = false;

        CreateInterstitialAd();
        PreloadInterstitial();

        if (enableLevelPlayTestSuite)
        {
            LevelPlay.LaunchTestSuite();
        }
    }

    private void OnLevelPlayInitFailed(LevelPlayInitError error)
    {
        levelPlayInitFailed = true;
        isLevelPlayInitialized = false;

        Debug.LogError("ANDROID TEST: LevelPlay initialization failed: " + error);
    }

    private void CreateInterstitialAd()
    {
        Debug.LogWarning("ANDROID TEST: CreateInterstitialAd wurde ausgeführt.");

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);

        interstitialAd.OnAdLoaded += OnInterstitialLoaded;
        interstitialAd.OnAdLoadFailed += OnInterstitialLoadFailed;
        interstitialAd.OnAdDisplayed += OnInterstitialDisplayed;
        interstitialAd.OnAdDisplayFailed += OnInterstitialDisplayFailed;
        interstitialAd.OnAdClosed += OnInterstitialClosed;
    }

    private void PreloadInterstitial()
    {
        if (!isLevelPlayInitialized)
        {
            Debug.LogWarning("ANDROID TEST: Interstitial kann nicht vorgeladen werden, LevelPlay ist nicht initialisiert.");
            return;
        }

        if (interstitialAd == null)
        {
            Debug.LogError("ANDROID TEST: Interstitial kann nicht vorgeladen werden, interstitialAd ist NULL.");
            return;
        }

        if (interstitialAd.IsAdReady())
        {
            Debug.LogWarning("ANDROID TEST: Interstitial ist bereits ready.");
            isInterstitialReady = true;
            isInterstitialLoading = false;
            return;
        }

        if (isInterstitialLoading)
        {
            Debug.LogWarning("ANDROID TEST: Interstitial lädt bereits.");
            return;
        }

        Debug.LogWarning("ANDROID TEST: Interstitial wird vorgeladen.");

        isInterstitialReady = false;
        isInterstitialLoading = true;

        interstitialAd.LoadAd();
    }

    public void ShowInterstitial()
    {
        Debug.LogWarning("ANDROID TEST: ShowInterstitial wurde aufgerufen.");

        if (levelPlayInitFailed)
        {
            Debug.LogError("ANDROID TEST: LevelPlay Init ist fehlgeschlagen. Interstitial kann nicht gezeigt werden.");
            return;
        }

        if (!isLevelPlayInitialized)
        {
            Debug.LogWarning("ANDROID TEST: LevelPlay ist noch nicht initialisiert. Interstitial wird später gezeigt, sobald möglich.");
            showInterstitialWhenLoaded = true;
            return;
        }

        if (interstitialAd == null)
        {
            Debug.LogError("ANDROID TEST: InterstitialAd ist NULL.");
            return;
        }

        if (isShowingInterstitial)
        {
            Debug.LogWarning("ANDROID TEST: Interstitial wird bereits angezeigt.");
            return;
        }

        bool isReady = interstitialAd.IsAdReady();
        Debug.LogWarning("ANDROID TEST: Interstitial IsAdReady = " + isReady);

        if (isReady)
        {
            ShowLoadedInterstitial();
            return;
        }

        Debug.LogWarning("ANDROID TEST: Interstitial ist noch nicht ready. Wird geladen und danach automatisch gezeigt.");

        showInterstitialWhenLoaded = true;

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(true);
        }

        PreloadInterstitial();
    }

    private void ShowLoadedInterstitial()
    {
        if (interstitialAd == null)
        {
            Debug.LogError("ANDROID TEST: ShowLoadedInterstitial fehlgeschlagen, interstitialAd ist NULL.");
            return;
        }

        if (!interstitialAd.IsAdReady())
        {
            Debug.LogWarning("ANDROID TEST: ShowLoadedInterstitial abgebrochen, Ad ist nicht mehr ready.");
            PreloadInterstitial();
            return;
        }

        Debug.LogWarning("ANDROID TEST: Interstitial wird jetzt angezeigt.");

        showInterstitialWhenLoaded = false;
        isInterstitialReady = false;
        isInterstitialLoading = false;
        isShowingInterstitial = true;

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(false);
        }

        interstitialAd.ShowAd();
    }

    private void OnInterstitialLoaded(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("ANDROID TEST: Interstitial loaded.");

        isInterstitialReady = true;
        isInterstitialLoading = false;

        if (showInterstitialWhenLoaded)
        {
            Debug.LogWarning("ANDROID TEST: Interstitial wurde angefordert und wird jetzt automatisch angezeigt.");
            ShowLoadedInterstitial();
        }
    }

    private void OnInterstitialLoadFailed(LevelPlayAdError error)
    {
        Debug.LogError("ANDROID TEST: Interstitial load failed: " + error);

        isInterstitialReady = false;
        isInterstitialLoading = false;
        showInterstitialWhenLoaded = false;

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(false);
        }
    }

    private void OnInterstitialDisplayed(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("ANDROID TEST: Interstitial displayed.");
        isShowingInterstitial = true;
    }

    private void OnInterstitialDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.LogError("ANDROID TEST: Interstitial display failed: " + error);

        isShowingInterstitial = false;
        isInterstitialReady = false;
        isInterstitialLoading = false;
        showInterstitialWhenLoaded = false;

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(false);
        }

        PreloadInterstitial();
    }

    private void OnInterstitialClosed(LevelPlayAdInfo adInfo)
    {
        Debug.LogWarning("ANDROID TEST: Interstitial closed. Lade nächste Werbung.");

        isShowingInterstitial = false;
        isInterstitialReady = false;
        isInterstitialLoading = false;
        showInterstitialWhenLoaded = false;

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(false);
        }

        PreloadInterstitial();
    }

    private IEnumerator WaitForFirstAdFreeTime()
    {
        Debug.LogWarning("ANDROID TEST: WaitForFirstAdFreeTime gestartet.");

        while (SaveManager.Instance != null && SaveManager.Instance.FirstSecondsWithoutAd > 0)
        {
            if (player != null && player.IsDead)
            {
                Debug.LogWarning("ANDROID TEST: Player ist tot während FirstSecondsWithoutAd. Speichern und abbrechen.");
                SaveManager.Instance.Save();
                yield break;
            }

            yield return new WaitForSeconds(1f);

            SaveManager.Instance.FirstSecondsWithoutAd--;
            SaveManager.Instance.Save();
        }

        Debug.LogWarning("ANDROID TEST: FirstSecondsWithoutAd ist vorbei.");
    }

    private void StartLevelAdCounter()
    {
        Debug.LogWarning("ANDROID TEST: StartLevelAdCounter.");

        if (levelAdRoutine != null)
        {
            StopCoroutine(levelAdRoutine);
        }

        levelAdRoutine = StartCoroutine(LevelAdCounterRoutine());
    }

    private IEnumerator LevelAdCounterRoutine()
    {
        Debug.LogWarning("ANDROID TEST: LevelAdCounterRoutine gestartet.");

        while (true)
        {
            yield return new WaitUntil(() => player != null && player.IsDead);

            Debug.LogWarning("ANDROID TEST: Player ist tot. HandleLevelFinishedForAd wird ausgeführt.");

            HandleLevelFinishedForAd();

            yield return new WaitUntil(() => player != null && !player.IsDead);

            Debug.LogWarning("ANDROID TEST: Player lebt wieder. Warte auf nächsten Tod.");
        }
    }

    private void HandleLevelFinishedForAd()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("ANDROID TEST: SaveManager.Instance ist NULL.");
            return;
        }

        Debug.LogWarning("ANDROID TEST: LevelsTillAdClip vorher = " + SaveManager.Instance.LevelsTillAdClip);

        if (SaveManager.Instance.LevelsTillAdClip > 1)
        {
            SaveManager.Instance.LevelsTillAdClip--;
            SaveManager.Instance.Save();

            Debug.LogWarning("ANDROID TEST: Noch keine Werbung. Neuer LevelsTillAdClip = " + SaveManager.Instance.LevelsTillAdClip);
            return;
        }

        SaveManager.Instance.LevelsTillAdClip = AmountOfLevelsTillAd;
        SaveManager.Instance.Save();

        Debug.LogWarning("ANDROID TEST: Interstitial soll jetzt gezeigt werden.");

        StartCoroutine(ShowInterstitialWhenPossible());
    }

    private IEnumerator ShowInterstitialWhenPossible()
    {
        Debug.LogWarning("ANDROID TEST: ShowInterstitialWhenPossible gestartet.");

        if (!isLevelPlayInitialized)
        {
            Debug.LogError("ANDROID TEST: Interstitial kann nicht gezeigt werden, LevelPlay ist nicht initialisiert.");
            yield break;
        }

        if (interstitialAd == null)
        {
            Debug.LogError("ANDROID TEST: Interstitial kann nicht gezeigt werden, interstitialAd ist NULL.");
            yield break;
        }

        if (isShowingInterstitial)
        {
            Debug.LogWarning("ANDROID TEST: Interstitial wird bereits angezeigt.");
            yield break;
        }

        if (interstitialAd.IsAdReady())
        {
            ShowLoadedInterstitial();
            yield break;
        }

        if (InputBlockingPanelGO != null)
        {
            InputBlockingPanelGO.SetActive(true);
        }

        Debug.LogWarning("ANDROID TEST: Interstitial nicht ready. Lade Ad und warte auf Timeout.");

        showInterstitialWhenLoaded = true;
        PreloadInterstitial();

        float elapsed = 0f;

        while (!isInterstitialReady && isInterstitialLoading && elapsed < interstitialLoadTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            ShowLoadedInterstitial();
        }
        else
        {
            Debug.LogWarning("ANDROID TEST: Interstitial war nach Timeout nicht bereit. Wird automatisch angezeigt, falls es noch lädt.");

            if (!isInterstitialLoading)
            {
                showInterstitialWhenLoaded = false;

                if (InputBlockingPanelGO != null)
                {
                    InputBlockingPanelGO.SetActive(false);
                }
            }
        }
    }

    public bool IsInterstitialReady()
    {
        return isLevelPlayInitialized &&
               interstitialAd != null &&
               interstitialAd.IsAdReady();
    }

    private void OnDestroy()
    {
        Debug.LogWarning("ANDROID TEST: AdManager OnDestroy.");

        LevelPlay.OnInitSuccess -= OnLevelPlayInitSuccess;
        LevelPlay.OnInitFailed -= OnLevelPlayInitFailed;

        if (interstitialAd != null)
        {
            interstitialAd.OnAdLoaded -= OnInterstitialLoaded;
            interstitialAd.OnAdLoadFailed -= OnInterstitialLoadFailed;
            interstitialAd.OnAdDisplayed -= OnInterstitialDisplayed;
            interstitialAd.OnAdDisplayFailed -= OnInterstitialDisplayFailed;
            interstitialAd.OnAdClosed -= OnInterstitialClosed;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}