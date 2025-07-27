using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown timeDropdown;
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    public void OnPlayPressed()
    {
        if (GameManager.Instance == null) return;

        int selectedIndex = timeDropdown.value;
        float[] durations = { 60f, 120f, 180f }; // 1min, 2min, 3min

        GameManager.Instance.GameDuration = durations[selectedIndex];

        SceneManager.LoadScene("Gameplay"); // Replace with your actual scene name
    }
    public void SetDifficultyFromDropdown()
    {
        GameManager.Instance.SetDifficulty((AIDifficulty)difficultyDropdown.value);
    }
}
