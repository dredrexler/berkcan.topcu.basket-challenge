// SwipeInputHandler.cs
using UnityEngine;
using UnityEngine.UI;

public class SwipeInputHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private ShotPowerSlider shotSlider;
    [SerializeField] private ShotManager shotManager;

    private Vector2 startTouch;
    private float swipeTimer;
    private bool isSwiping;
    private bool shotFired;
    private const float maxSwipeDuration = 0.75f;
    private const float swipeDistanceForFull = 500f; // adjust if you like

    void OnEnable()
    {
        swipeTimer = 0f;
        isSwiping = false;
        shotFired = false;
    }

    void Update()
    {
        if (shotManager.IsShotInProgress())
            return;

        // --- 1) Mobile touch input ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeginSwipe(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    ContinueSwipe(touch.position, Time.deltaTime);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndSwipe();
                    break;
            }
        }
        // --- 2) Mouse fallback ---
        else
        {
            if (Input.GetMouseButtonDown(0))
                BeginSwipe(Input.mousePosition);

            if (isSwiping && !shotFired)
                ContinueSwipe((Vector2)Input.mousePosition, Time.deltaTime);

            if (Input.GetMouseButtonUp(0) && isSwiping && !shotFired)
                EndSwipe();
        }
    }

    private void BeginSwipe(Vector2 inputPos)
    {
        startTouch   = inputPos;
        swipeTimer   = 0f;
        isSwiping    = true;
        shotFired    = false;
        shotSlider.StartDrag();
    }

    private void ContinueSwipe(Vector2 currentPos, float deltaTime)
    {
        swipeTimer += deltaTime;

        float deltaY = currentPos.y - startTouch.y;
        float normalized = Mathf.Clamp01(deltaY / swipeDistanceForFull);

        // only ever increase
        if (normalized > powerSlider.value)
            powerSlider.value = normalized;

        // auto‑fire if held too long
        if (swipeTimer >= maxSwipeDuration)
            EndSwipe();
    }

    private void EndSwipe()
    {
        shotSlider.StopDrag();
        shotFired = true;
        isSwiping = false;
    }
}
