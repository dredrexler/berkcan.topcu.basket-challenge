using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CountdownManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public float startDelay = 0f;
    [SerializeField] private TextMeshProUGUI modesMessageText;
    [SerializeField] private GameObject panelRoot;

    [Header("Events")]
    public UnityEvent OnCountdownFinished;

    void Start()
    {
        countdownPanel.SetActive(false);
        panelRoot.SetActive(false);
        StartCoroutine(DoCountdown());
    }

    private IEnumerator DoCountdown()
    {
        yield return new WaitForSeconds(startDelay);
        countdownPanel.SetActive(true);
        panelRoot.SetActive(true);

        // Build the message
        string msg = "";

        // Always show base info
        msg += "Slide To Make Shots\n<color=green>Green</color>: Perfect  <color=purple>Purple</color>: Backboard & Bonus\n";

        // Add mode-specific info
        if (GameManager.Instance.FreezeModeEnabled)
            msg += "\n<color=blue>Freeze Mode</color>: Make 3 Perfect Shots in a Row To Freeze the Opponent!";
        if (GameManager.Instance.IsReplayEnabled)
            msg += "\n<color=yellow>Replay</color>: Score Bonuses To Replay Your Shot!";
        if (GameManager.Instance.IsClutchTimeEnabled)
            msg += "\n<color=red>Clutch Time</color>: Points are x2 in Last 20 Seconds!";

        modesMessageText.text = msg;

        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownPanel.SetActive(false);
        panelRoot.SetActive(false);
        OnCountdownFinished?.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            GameManager.Instance.StartTimer();
        }

        
    }
}
