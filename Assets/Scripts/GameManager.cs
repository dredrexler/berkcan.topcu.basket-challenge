using UnityEngine;


// Lives across scenes and keeps your session data (score, coins).

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int TotalScore { get; private set; }
    public int Coins { get; private set; }

    private void Awake()
    {
        // Enforce a single instance that survives scene loads
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Reset per-run data (called when starting a new gameplay session).
    public void ResetRun()
    {
        TotalScore = 0;
        Coins = 0;
    }

    // Add points during gameplay.
    public void AddScore(int points)
    {
        TotalScore += points;
    }

    // Add coins during gameplay.
    public void AddCoins(int amount)
    {
        Coins += amount;
    }
}
