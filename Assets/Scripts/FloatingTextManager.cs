using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;
    [SerializeField] private TextMeshProUGUI floatingText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float moveSpeed = 30f;

    private Coroutine currentRoutine;
    private Vector2 originalPosition;

    private void Start()
    {
        if (floatingText != null)
        {
            originalPosition = floatingText.rectTransform.anchoredPosition;
            floatingText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message, Color color)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        floatingText.text = message;
        floatingText.color = color;
        floatingText.rectTransform.anchoredPosition = originalPosition;
        floatingText.gameObject.SetActive(true);

        currentRoutine = StartCoroutine(AnimateFloatingText());
    }

    private IEnumerator AnimateFloatingText()
    {
        float elapsed = 0f;
        Color startColor = floatingText.color;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;

            // Move up
            float offset = elapsed * moveSpeed;
            floatingText.rectTransform.anchoredPosition = originalPosition + new Vector2(0f, offset);

            // Fade out
            float alpha = Mathf.Lerp(1f, 0f, elapsed / displayDuration);
            floatingText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        floatingText.gameObject.SetActive(false);
    }
}
