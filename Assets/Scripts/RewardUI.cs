using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


// Shows final results and lets the user replay or go to main menu.

public class RewardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToMainButton;
    public TMP_Text resultText;

    private void Start()
    {
        scoreText.text = $"YOU: {GameManager.Instance.TotalScore}";
        scoreText.color = GameManager.Instance.TotalScore > GameManager.Instance.AIScore ? Color.green : Color.red;
        aiScoreText.text = $"AI: {GameManager.Instance.AIScore}";
        aiScoreText.color = GameManager.Instance.AIScore > GameManager.Instance.TotalScore ? Color.green : Color.red;
        coinsText.text = $"Coins: {GameManager.Instance.Coins}";
        coinsText.color = GameManager.Instance.Coins > 0 ? Color.yellow : Color.white;

        playAgainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("Gameplay"));

        backToMainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("MainMenu"));
            
        switch (GameManager.Instance.Result)
        {
            case GameResult.Win:
                resultText.text = "YOU WIN!";
                resultText.color = Color.green;
                break;
            case GameResult.Lose:
                resultText.text = "YOU LOSE!";
                resultText.color = Color.red;
                break;
            case GameResult.Draw:
                resultText.text = "DRAW!";
                break;
            default:
                resultText.text = "";
                break;
        }
    }
}
