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
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToMainButton;
    public TMP_Text resultText;

    private void Start()
    {
        scoreText.text = $"Final Score: {GameManager.Instance.TotalScore}";
        coinsText.text = $"Coins: {GameManager.Instance.Coins}";

        playAgainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("Gameplay"));

        backToMainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("MainMenu"));
            
        switch (GameManager.Instance.Result)
        {
            case GameResult.Win:
                resultText.text = "You Win!";
                break;
            case GameResult.Lose:
                resultText.text = "You Lose!";
                break;
            case GameResult.Draw:
                resultText.text = "Draw!";
                break;
            default:
                resultText.text = "";
                break;
        }
    }
}
