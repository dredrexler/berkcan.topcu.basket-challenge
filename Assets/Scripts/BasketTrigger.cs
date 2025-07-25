using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasketTrigger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
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
                UpdateScoreUI();
                break;
            case ShotType.Rim:
                GameManager.Instance.AddScore(2);
                UpdateScoreUI();
                break;
            case ShotType.Backboard:
                GameManager.Instance.AddScore(2); 
                UpdateScoreUI();
                break;
        }

        status.hasScored = true;
    }
    
    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {GameManager.Instance.TotalScore}";
    }
}
