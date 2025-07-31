using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;   
public enum GameResult
{
    None,
    Win,
    Lose,
    Draw
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Campaign Settings")]
    public bool InCampaign = false;
    public int CampaignIndex = 0;
    public string[] CampaignScenes;
    public AIDifficulty[] CampaignDifficulties;

    [Header("Gameplay Settings")]
    public float GameDuration = 60f; // Default duration in seconds
    public float TimeRemaining { get; private set; }
    public bool IsTimerRunning { get; private set; }

    public int TotalScore { get; private set; } // Player score
    public int AIScore { get; private set; }    // AI score
    public int Coins { get; private set; }
    public GameResult Result { get; private set; } = GameResult.None;
    public AIDifficulty SelectedDifficulty { get; private set; } = AIDifficulty.Easy;
    public bool IsChangingPosition { get; private set; }

    [Header("Runtime Debug")]
    [SerializeField]
    private bool _gameStarted;
    public bool GameStarted { get => _gameStarted; private set => _gameStarted = value; }

    [Header("AI Debug (Editor Only)")]
    [SerializeField] private AIShotManager aiShotManager;
    [SerializeField] private TMP_Dropdown aiShotTypeDropdown;
    [SerializeField] private Button aiDebugShootButton;

    public bool IsClutchTimeEnabled = false;
    public bool IsClutchTimeActive => IsClutchTimeEnabled && TimeRemaining <= 20f && IsTimerRunning;

    public bool IsReplayEnabled = false;
    public bool IsInReplay = false;
    public bool FreezeModeEnabled = false;

    public string LastPlayedScene { get; private set; } = "Gameplay"; // default fallback

    void Start()
    {
        if (aiShotManager != null && aiShotTypeDropdown != null && aiDebugShootButton != null)
        {
            aiDebugShootButton.onClick.AddListener(() =>
            {
                // Cast dropdown index to ShotType enum (ensure your dropdown options match)
                ShotType debugType = (ShotType)aiShotTypeDropdown.value;
                aiShotManager.StartShot(debugType);
            });
        }
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLastPlayedScene(string sceneName)
    {
        LastPlayedScene = sceneName;
    }

    // ---------------- Campaign Flow ----------------
    public void StartCampaign()
    {
        if (CampaignScenes == null || CampaignDifficulties == null ||
            CampaignScenes.Length != CampaignDifficulties.Length || CampaignScenes.Length == 0)
        {
            Debug.LogError("Campaign arrays not configured or mismatch length.");
            return;
        }
        InCampaign = true;
        CampaignIndex = 0;
        LoadCampaignLevel();
    }

    private void LoadCampaignLevel()
    {
        // set difficulty for this level
        SetDifficulty(CampaignDifficulties[CampaignIndex]);
        // load the gameplay scene
        SceneManager.LoadScene(CampaignScenes[CampaignIndex]);
    }

    public void NextCampaignLevel()
    {
        if (!InCampaign) return;
        if (CampaignIndex < CampaignScenes.Length - 1)
        {
            CampaignIndex++;
            LoadCampaignLevel();
        }
        else
        {
            // finished last level
            InCampaign = false;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void RestartCampaignLevel()
    {
        if (!InCampaign) { SceneManager.LoadScene("MainMenu"); return; }
        // reload the same index
        SceneManager.LoadScene(CampaignScenes[CampaignIndex]);
    }

    public void ExitToMainMenu()
    {
        InCampaign = false;
        SceneManager.LoadScene("MainMenu");
    }

    // ---------------- Gameplay Flow ----------------
    public void StartGame()
    {
        GameStarted = true;
    }

    public void SetPositionLock(bool state)
    {
        IsChangingPosition = state;
    }

    public void StartTimer()
    {
        TimeRemaining = GameDuration;
        IsTimerRunning = true;
    }

    public void ResumeTimer()
    {
        // Don't reset TimeRemaining!
        IsTimerRunning = true;
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
    }

    private void Update()
    {
        if (IsTimerRunning)
        {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0f)
            {
                TimeRemaining = 0f;
                IsTimerRunning = false;
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over - Timer ended!");

        // Determine win/lose/draw
        if (TotalScore > AIScore)
            Result = GameResult.Win;
        else if (TotalScore < AIScore)
            Result = GameResult.Lose;
        else
            Result = GameResult.Draw;

        AddCoins(TotalScore);
        // After gameplay ends, load Reward

        SceneManager.LoadScene("Reward");
    }


    public void ResetRun()
    {
        TotalScore = 0;
        AIScore = 0;
        Coins = 0;
        TimeRemaining = GameDuration;
        GameStarted = false;
    }

    public void AddScore(int points)
    {
        TotalScore += points;
    }

    public void AddAIScore(int aiPoints)
    {
        AIScore += aiPoints;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
    }

    public void SetDifficulty(AIDifficulty difficulty)
    {
        SelectedDifficulty = difficulty;
    }

    public void StartFreezeGame()
    {
        SceneManager.LoadScene("FreezeMode");
    }


}
