using UnityEngine;
using UnityEngine.UI;

public class SwipeInputHandler : MonoBehaviour
{
    [SerializeField] private Slider powerSlider;
    [SerializeField] private ShotPowerSlider shotSlider;
    [SerializeField] private ShotManager shotManager;

    private Vector2 startTouch;
    private bool isSwiping = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouch = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            float swipeDistance = Input.mousePosition.y - startTouch.y;
            float normalized = Mathf.Clamp01(swipeDistance / 500f); // 500f = swipe height sensitivity
            shotSlider.SetFill(normalized);
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            shotSlider.TriggerShot();
            isSwiping = false;
        }
    }

}
