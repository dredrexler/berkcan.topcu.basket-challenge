using UnityEngine;
using UnityEngine.SceneManagement;
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

    public float GameDuration = 60f; // Default duration in seconds
    public float TimeRemaining { get; private set; }
    public bool IsTimerRunning { get; private set; }

    public int TotalScore { get; private set; } // Player score
    public int AIScore { get; private set; }    // AI score
    public int Coins { get; private set; }
    public GameResult Result { get; private set; } = GameResult.None;
    public AIDifficulty SelectedDifficulty { get; private set; } = AIDifficulty.Easy;
    public bool IsChangingPosition { get; private set; }
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
    public void SetPositionLock(bool state)
    {
        IsChangingPosition = state;
    }

    public void StartTimer()
    {
        TimeRemaining = GameDuration;
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
        int coinsToGive = TotalScore;
        AddCoins(coinsToGive);
        SceneManager.LoadScene("Reward");
    }

    public void ResetRun()
    {
        TotalScore = 0;
        AIScore = 0;
        Coins = 0;
        TimeRemaining = GameDuration;
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
}
