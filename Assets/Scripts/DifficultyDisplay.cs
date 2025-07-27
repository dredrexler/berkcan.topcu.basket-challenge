using TMPro;
using UnityEngine;

public class DifficultyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI difficultyText;

    private void Start()
    {
        if (GameManager.Instance != null && difficultyText != null)
        {
            string difficultyName = GameManager.Instance.SelectedDifficulty.ToString();
            difficultyText.text = "Difficulty: " + difficultyName;
        }
    }
}
