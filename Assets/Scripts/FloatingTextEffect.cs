using UnityEngine;
using TMPro;

public class FloatingTextEffect : MonoBehaviour
{
    private float duration = 2f;
    private float speed = 0.5f;
    private TextMeshProUGUI text;
    private Color originalColor;
    private float timer = 0f;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            originalColor = text.color;
            // Force alpha to be fully visible at start
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        timer += Time.deltaTime;

        if (text != null)
        {
            float fade = Mathf.Clamp01(1f - (timer / duration)); // fade from 1 to 0 over duration
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, fade);
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
