using UnityEngine;
using TMPro;

public class AIBasketTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI aiScoreText;

    [Header("Scoring Helpers")]
    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private AIFireballManager fireballManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("AIBall")) return;

        var status = other.GetComponent<BallStatus>();
        if (status == null || status.hasScored) return;

        Debug.Log("AI Ball entered the basket!");

        // Determine base points
        int basePoints;
        switch (status.shotType)
        {
            case ShotType.Perfect:
                basePoints = 3;
                break;

            case ShotType.Rim:
                basePoints = 2;
                break;

            case ShotType.Backboard:
                int bonus = backboardBonusManager.GetBonusPoints();
                basePoints = bonus > 0 ? bonus : 2;
                break;

            default:
                basePoints = 0;
                break;
        }

        // Apply fireball multiplier
        int totalPoints = fireballManager.ApplyMultiplier(basePoints);

        // Award AI score
        GameManager.Instance.AddAIScore(totalPoints);

        // Advance the fireball streak
        fireballManager.OnMake();

        // Update AI score UI
        UpdateScoreUI();

        status.hasScored = true;
    }


    public void OnAIMiss()
    {
        fireballManager.OnMiss();
    }

    private void UpdateScoreUI()
    {
        aiScoreText.text = $"{GameManager.Instance.AIScore}";
    }
}
