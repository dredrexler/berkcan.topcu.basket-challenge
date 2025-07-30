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
    [SerializeField] private CountdownManager countdownManager;
    [SerializeField] private CanvasGroup gameplayUI1;
    [SerializeField] private CanvasGroup gameplayUI2;

    private void Start()
    {
        // Start a fresh run every time we enter Gameplay
        GameManager.Instance.ResetRun();
        UpdateScoreUI();

        GameManager.Instance.StopTimer();
        // don’t start the timer yet — wait for the countdown
        addTwoPointsButton.interactable = false;
        finishRoundButton.interactable = false;

        // Hook test buttons
        addTwoPointsButton.onClick.AddListener(() => AddPoints(2));
        finishRoundButton.onClick.AddListener(FinishRound);

        // When countdown finishes, start timer and enable buttons
        countdownManager.OnCountdownFinished.AddListener(() =>
        {
            GameManager.Instance.StartTimer();
            addTwoPointsButton.interactable = true;
            finishRoundButton.interactable = true;
        });

        // hide & disable all gameplay UI before the countdown finishes
        gameplayUI1.alpha = 0f;
        gameplayUI1.interactable = false;
        gameplayUI1.blocksRaycasts = false;
        gameplayUI2.alpha = 0f;
        gameplayUI2.interactable = false;
        gameplayUI2.blocksRaycasts = false;

        // when countdown ends, show the UI
        countdownManager.OnCountdownFinished.AddListener(EnableGameplayUI);
    }

    private void EnableGameplayUI()
    {
        gameplayUI1.alpha = 1f;
        gameplayUI1.interactable = true;
        gameplayUI1.blocksRaycasts = true;
        gameplayUI2.alpha = 1f;
        gameplayUI2.interactable = true;
        gameplayUI2.blocksRaycasts = true;
    }

    private void AddPoints(int amount)
    {
        GameManager.Instance.AddScore(amount);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{GameManager.Instance.TotalScore}";
    }

  
      
    private void FinishRound()
    {
        int coinsToGive = GameManager.Instance.TotalScore;
        GameManager.Instance.AddCoins(coinsToGive);

        // Load Reward scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Reward");
    }
}
