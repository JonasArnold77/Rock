using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MobileDebugConsole : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private TMP_Text consoleLinePrefab;

    [Header("Settings")]
    [SerializeField] private bool showOnStart = true;
    [SerializeField] private int maxLines = 80;

    [Header("Log Types")]
    [SerializeField] private bool showLogs = false;
    [SerializeField] private bool showWarnings = true;
    [SerializeField] private bool showErrors = true;
    [SerializeField] private bool showExceptions = true;

    private readonly Queue<GameObject> spawnedLines = new Queue<GameObject>();
    private readonly Queue<LogEntry> pendingLogs = new Queue<LogEntry>();

    private struct LogEntry
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public LogEntry(string condition, string stackTrace, LogType type)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (consolePanel != null)
        {
            consolePanel.SetActive(showOnStart);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLogThreaded;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLogThreaded;
    }

    private void Update()
    {
        ProcessPendingLogs();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleConsole();
        }

        if (Input.touchCount == 3)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                ToggleConsole();
            }
        }
    }

    private void HandleLogThreaded(string condition, string stackTrace, LogType type)
    {
        if (!ShouldShowLog(type))
        {
            return;
        }

        lock (pendingLogs)
        {
            pendingLogs.Enqueue(new LogEntry(condition, stackTrace, type));
        }
    }

    private void ProcessPendingLogs()
    {
        lock (pendingLogs)
        {
            while (pendingLogs.Count > 0)
            {
                LogEntry entry = pendingLogs.Dequeue();
                CreateLogLine(entry.condition, entry.stackTrace, entry.type);
            }
        }
    }

    private bool ShouldShowLog(LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                return showLogs;

            case LogType.Warning:
                return showWarnings;

            case LogType.Error:
                return showErrors;

            case LogType.Exception:
                return showExceptions;

            default:
                return false;
        }
    }

    private void CreateLogLine(string message, string stackTrace, LogType type)
    {
        if (consoleLinePrefab == null || contentRoot == null)
        {
            return;
        }

        TMP_Text newLine = Instantiate(consoleLinePrefab, contentRoot);
        newLine.gameObject.SetActive(true);

        newLine.text = BuildMessage(message, stackTrace, type);

        spawnedLines.Enqueue(newLine.gameObject);

        while (spawnedLines.Count > maxLines)
        {
            GameObject oldLine = spawnedLines.Dequeue();

            if (oldLine != null)
            {
                Destroy(oldLine);
            }
        }

        ScrollToBottom();
    }

    private string BuildMessage(string message, string stackTrace, LogType type)
    {
        string prefix = GetPrefix(type);

        string finalMessage = $"{prefix} {message}";

        if (type == LogType.Error || type == LogType.Exception)
        {
            finalMessage += $"\n<size=70%>{stackTrace}</size>";
        }

        return finalMessage;
    }

    private string GetPrefix(LogType type)
    {
        switch (type)
        {
            case LogType.Log:
                return "<color=white>[LOG]</color>";

            case LogType.Warning:
                return "<color=yellow>[WARNING]</color>";

            case LogType.Error:
                return "<color=red>[ERROR]</color>";

            case LogType.Exception:
                return "<color=red>[EXCEPTION]</color>";

            default:
                return "<color=white>[UNKNOWN]</color>";
        }
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }

        Canvas.ForceUpdateCanvases();
    }

    public void ToggleConsole()
    {
        if (consolePanel == null)
        {
            return;
        }

        consolePanel.SetActive(!consolePanel.activeSelf);
    }

    public void ShowConsole()
    {
        if (consolePanel != null)
        {
            consolePanel.SetActive(true);
        }
    }

    public void HideConsole()
    {
        if (consolePanel != null)
        {
            consolePanel.SetActive(false);
        }
    }

    public void ClearConsole()
    {
        while (spawnedLines.Count > 0)
        {
            GameObject line = spawnedLines.Dequeue();

            if (line != null)
            {
                Destroy(line);
            }
        }

        ScrollToBottom();
    }
}