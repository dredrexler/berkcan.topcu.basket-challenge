using UnityEngine;
using UnityEngine.UI;

public class FreezeModeUI : MonoBehaviour
{
    [SerializeField] private Slider freezeSlider;

    private float freezeDuration;
    private float timeLeft;
    private bool freezeActive = false;

    void Start()
    {
        freezeSlider.gameObject.SetActive(false);
    }

    // Call this when Freeze Mode starts
    public void ActivateFreezeSlider(float duration)
    {
        freezeDuration = duration;
        timeLeft = duration;
        freezeActive = true;
        freezeSlider.maxValue = freezeDuration;
        freezeSlider.value = freezeDuration;
        freezeSlider.gameObject.SetActive(true);
    }

    // Call this when Freeze Mode ends
    public void HideFreezeSlider()
    {
        freezeActive = false;
        freezeSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!freezeActive) return;

        timeLeft -= Time.deltaTime;
        freezeSlider.value = Mathf.Clamp(timeLeft, 0, freezeDuration);

        if (timeLeft <= 0)
        {
            freezeActive = false;
            freezeSlider.gameObject.SetActive(false);
        }
    }
}
