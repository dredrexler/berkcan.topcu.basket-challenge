using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (!GameManager.Instance.IsTimerRunning)
            return;

        float timeLeft = GameManager.Instance.TimeRemaining;
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
