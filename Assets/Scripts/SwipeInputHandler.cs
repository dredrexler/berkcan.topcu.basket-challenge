// SwipeInputHandler.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

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
    private const float swipeDistanceForFull = 750f;

    void OnEnable()
    {
        swipeTimer = 0f;
        isSwiping = false;
        shotFired = false;
    }

    void Update()
    {
        if (!GameManager.Instance.GameStarted) return;
        if (shotManager.IsShotInProgress())
            return;

#if UNITY_ANDROID || UNITY_IOS
        // Touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!IsPointerOverUIButton(touch.position) && !IsPointerOverUIDropdown(touch.position))
                        BeginSwipe(touch.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isSwiping && !shotFired)
                        ContinueSwipe(touch.position, Time.deltaTime);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isSwiping && !shotFired)
                        EndSwipe();
                    break;
            }
        }
#endif

        // Mouse input 
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (!IsPointerOverUIButton(mousePos) && !IsPointerOverUIDropdown(mousePos))
                BeginSwipe(mousePos);
        }

        if (isSwiping && !shotFired)
        {
            ContinueSwipe(Input.mousePosition, Time.deltaTime);
        }

        if (Input.GetMouseButtonUp(0) && isSwiping && !shotFired)
        {
            EndSwipe();
        }
    }

    private void BeginSwipe(Vector2 inputPos)
    {
        startTouch = inputPos;
        swipeTimer = 0f;
        isSwiping = true;
        shotFired = false;
        shotSlider.StartDrag();
    }

    private void ContinueSwipe(Vector2 currentPos, float deltaTime)
    {
        swipeTimer += deltaTime;
        float deltaY = currentPos.y - startTouch.y;
        float normalized = Mathf.Clamp01(deltaY / swipeDistanceForFull);

        if (normalized > powerSlider.value)
            powerSlider.value = normalized;

        if (swipeTimer >= maxSwipeDuration)
            EndSwipe();
    }

    private void EndSwipe()
    {
        shotSlider.StopDrag();
        shotFired = true;
        isSwiping = false;
    }


    /// Returns true if the given screen point is over any Button UI.

    private bool IsPointerOverUIButton(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        // Set up a PointerEventData with current pointer position
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        // Raycast into the UI
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Check if any hit object has a Button component
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }
        return false;
    }
    
    private bool IsPointerOverUIDropdown(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        // Set up a PointerEventData with current pointer position
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        // Raycast into the UI
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Check if any hit object has a Dropdown component
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<TMP_Dropdown>() != null)
                return true;
        }
        return false;
    }
}
