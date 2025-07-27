using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIBasketTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private FloatingTextManager floatingTextManager;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("AIBall"))
            return;

        BallStatus status = other.GetComponent<BallStatus>();
        if (status == null)
            return;

        if (status.hasScored)
            return; // prevent double-scoring

        Debug.Log("AI Ball entered the basket!");

        // Add score based on shot type
        switch (status.shotType)
        {
            case ShotType.Perfect:
                GameManager.Instance.AddAIScore(3);
                UpdateScoreUI();
                break;
            case ShotType.Rim:
                GameManager.Instance.AddAIScore(2);
                UpdateScoreUI();
                break;
            case ShotType.Backboard:
                int bonus = backboardBonusManager.GetBonusPoints();
                if (bonus > 0)
                {
                    GameManager.Instance.AddAIScore(bonus);
                    UpdateScoreUI();
                }
                else
                {
                    GameManager.Instance.AddAIScore(2);
                    UpdateScoreUI();
                }
            break;
        }
        
        status.hasScored = true;
    }
    
    private void UpdateScoreUI()
    {
        aiScoreText.text = $"AI: {GameManager.Instance.AIScore}";
    }
}
