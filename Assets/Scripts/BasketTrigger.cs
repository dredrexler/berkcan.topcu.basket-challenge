using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasketTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private FloatingTextManager floatingTextManager;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        BallStatus status = other.GetComponent<BallStatus>();
        if (status == null)
            return;

        if (status.hasScored)
            return; // prevent double-scoring

        Debug.Log("Ball entered the basket!");

        // Add score based on shot type
        switch (status.shotType)
        {
            case ShotType.Perfect:
                GameManager.Instance.AddScore(3);
                floatingTextManager.ShowMessage("Perfect Shot!\n+3 Points!", Color.green);
                UpdateScoreUI();
                break;
            case ShotType.Rim:
                GameManager.Instance.AddScore(2);
                floatingTextManager.ShowMessage("+2 Points!", Color.grey);
                UpdateScoreUI();
                break;
            case ShotType.Backboard:
                int bonus = backboardBonusManager.GetBonusPoints();
                if (bonus > 0)
                {
                    GameManager.Instance.AddScore(bonus);
                    floatingTextManager.ShowMessage($"Backboard Bonus! +{bonus} Points!", Color.magenta);
                    UpdateScoreUI();
                }
                else
                {
                    GameManager.Instance.AddScore(2);
                    floatingTextManager.ShowMessage("+2 Points!", Color.white);
                    UpdateScoreUI();
                }
            break;
        }
        
        status.hasScored = true;
    }
    
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {GameManager.Instance.TotalScore}";
    }
}
