using UnityEngine;
using UnityEngine.UI;

public class SwipeInputHandler : MonoBehaviour
{
    [SerializeField] private Slider powerSlider;
    [SerializeField] private ShotPowerSlider shotSlider;
    [SerializeField] private ShotManager shotManager;

    private Vector2 startTouch;
    private float swipeTimer = 0f;
    private bool isSwiping = false;
    private bool shotFired = false;
    private const float maxSwipeDuration = 0.75f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
            swipeTimer = 0f;
            isSwiping = true;
            shotFired = false;
        }

        if (isSwiping && !shotFired)
        {
            swipeTimer += Time.deltaTime;

            Vector2 currentTouch = Input.mousePosition;
            float swipeDistance = currentTouch.y - startTouch.y;
            float normalized = Mathf.Clamp01(swipeDistance / 500f);

            // Prevent slider from decreasing
            if (normalized > powerSlider.value)
            {
                powerSlider.value = normalized;
            }


            if (swipeTimer >= maxSwipeDuration)
            {
                FireShot();
            }
        }

        if (Input.GetMouseButtonUp(0) && isSwiping && !shotFired)
        {
            FireShot();
        }
    }

    private void FireShot()
    {
        float finalValue = powerSlider.value;
        ShotType result = shotSlider.GetShotTypeFromValue(finalValue);
        shotManager.StartShot(result);

        shotFired = true;
        isSwiping = false;
        powerSlider.value = 0;
    }
}
