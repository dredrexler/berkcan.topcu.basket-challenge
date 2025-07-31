using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button nextGameButton;
    [SerializeField] private Button backToMainButton;
    [SerializeField] private DribbleBall dribbleBall;
    [SerializeField] private AIDribbleBall aidribbleBall;

    void Start()
    {
        dribbleBall?.StartDribble();
        aidribbleBall?.StartDribble();
        // Display scores
        int player = GameManager.Instance.TotalScore;
        int ai     = GameManager.Instance.AIScore;
        scoreText.text   = player.ToString();
        aiScoreText.text = ai.ToString();
        scoreText.color  = player > ai ? Color.green : Color.red;
        aiScoreText.color= ai > player ? Color.green : Color.red;

        // Display coins
        int coins = GameManager.Instance.Coins;
        coinsText.text   = $"Coins: {coins}";
        coinsText.color  = coins > 0 ? Color.yellow : Color.white;

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
        }

        // Play Again: either restart campaign or load quick‐play
        playAgainButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.InCampaign)
                GameManager.Instance.RestartCampaignLevel();
            else
                SceneManager.LoadScene("Gameplay");
        });

        // Next Game: only show if in campaign, just won, and not last level
        bool canAdvance =
            GameManager.Instance.InCampaign
         && GameManager.Instance.Result == GameResult.Win
         && GameManager.Instance.CampaignIndex < GameManager.Instance.CampaignScenes.Length - 1;

        nextGameButton.gameObject.SetActive(canAdvance);
        if (canAdvance)
            nextGameButton.onClick.AddListener(() =>
                GameManager.Instance.NextCampaignLevel()
            );

        // Back to Main Menu
        backToMainButton.onClick.AddListener(() =>
        {
            GameManager.Instance.InCampaign = false;
            SceneManager.LoadScene("MainMenu");
        });
    }
}
