using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Temporary controller to test flow: lets you add points and finish the round.
// Replace the scoring button with the real swipe/shot logic later.

public class GameplayController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button addTwoPointsButton;   // test button
    [SerializeField] private Button finishRoundButton;    // goes to Reward

    private void Start()
    {
        // Start a fresh run every time we enter Gameplay
        GameManager.Instance.ResetRun();
        UpdateScoreUI();
        GameManager.Instance.StartTimer();
        // Hook test buttons
        addTwoPointsButton.onClick.AddListener(() => AddPoints(2));
        finishRoundButton.onClick.AddListener(FinishRound);
    }

    private void AddPoints(int amount)
    {
        GameManager.Instance.AddScore(amount);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {GameManager.Instance.TotalScore}";
    }

  
      
    private void FinishRound()
    {
        int coinsToGive = GameManager.Instance.TotalScore;
        GameManager.Instance.AddCoins(coinsToGive);

        // Load Reward scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Reward");
    }
}
