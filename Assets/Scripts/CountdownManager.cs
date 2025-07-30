using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events; 

public class CountdownManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public float startDelay = 0f;

    [Header("Events")]
    public UnityEvent OnCountdownFinished;

    void Start()
    {
        countdownPanel.SetActive(false);
        StartCoroutine(DoCountdown());
    }

    private IEnumerator DoCountdown()
    {
        yield return new WaitForSeconds(startDelay);
        countdownPanel.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownPanel.SetActive(false);
        GameManager.Instance.StartGame();
        GameManager.Instance.StartTimer();

        OnCountdownFinished?.Invoke();
    }
}
