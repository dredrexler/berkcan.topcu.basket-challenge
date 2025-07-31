using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameplayController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button addTwoPointsButton;   // test button
    [SerializeField] private Button finishRoundButton;    // goes to Reward
    [SerializeField] private CountdownManager countdownManager;
    [SerializeField] private CanvasGroup gameplayUI1;
    [SerializeField] private CanvasGroup gameplayUI2;
    [SerializeField] private TextMeshProUGUI clutchTimeText;
    private bool clutchTimeDisplayed = false;

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
        clutchTimeText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!clutchTimeDisplayed && GameManager.Instance.IsClutchTimeEnabled && GameManager.Instance.TimeRemaining <= 20f)
        {
            ActivateClutchTime();
        }
    }

    private void ActivateClutchTime()
    {
        clutchTimeText.gameObject.SetActive(true);
        clutchTimeText.text = "CLUTCH TIME x2 POINTS";
        clutchTimeText.color = Color.red;

        clutchTimeDisplayed = true;
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
