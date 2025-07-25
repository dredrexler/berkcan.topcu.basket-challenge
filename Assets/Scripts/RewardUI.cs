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

    private void Start()
    {
        scoreText.text = $"Final Score: {GameManager.Instance.TotalScore}";
        coinsText.text = $"Coins: {GameManager.Instance.Coins}";

        playAgainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("Gameplay"));

        backToMainButton.onClick.AddListener(() =>
            SceneManager.LoadScene("MainMenu"));
    }
}
