using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// Shows final results and lets the user replay, go to main menu, or advance in Campaign.
public class RewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToMainButton;
    [SerializeField] private Button nextGameButton;  // New for Campaign

    void Start()
    {
        // Display scores
        int player = GameManager.Instance.TotalScore;
        int ai     = GameManager.Instance.AIScore;
        scoreText.text  = player.ToString();
        scoreText.color = player > ai ? Color.green : Color.red;
        aiScoreText.text  = ai.ToString();
        aiScoreText.color = ai > player ? Color.green : Color.red;

        // Display coins
        int coins = GameManager.Instance.Coins;
        coinsText.text  = $"Coins: {coins}";
        coinsText.color = coins > 0 ? Color.yellow : Color.white;

        // Display result message
        switch (GameManager.Instance.Result)
        {
            case GameResult.Win:
                resultText.text  = "YOU WIN!";
                resultText.color = Color.green;
                break;
            case GameResult.Lose:
                resultText.text  = "YOU LOSE!";
                resultText.color = Color.red;
                break;
            case GameResult.Draw:
                resultText.text  = "DRAW!";
                resultText.color = Color.white;
                break;
            default:
                resultText.text = string.Empty;
                break;
        }

        // Play Again: in Campaign restart level, otherwise load standard gameplay
        playAgainButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.InCampaign)
                GameManager.Instance.RestartCampaignLevel();
            else
                SceneManager.LoadScene("Gameplay");
        });

        // Back to Main Menu
        backToMainButton.onClick.AddListener(() =>
        {
            GameManager.Instance.InCampaign = false;
            SceneManager.LoadScene("MainMenu");
        });

        // Next Game: only when in Campaign, on a Win, and not final stage
        bool showNext = GameManager.Instance.InCampaign
                      && GameManager.Instance.Result == GameResult.Win
                      && GameManager.Instance.CampaignIndex < GameManager.Instance.CampaignScenes.Length - 1;
        nextGameButton.gameObject.SetActive(showNext);
        if (showNext)
        {
            nextGameButton.onClick.AddListener(() =>
            {
                GameManager.Instance.NextCampaignLevel();
            });
        }
    }
}
