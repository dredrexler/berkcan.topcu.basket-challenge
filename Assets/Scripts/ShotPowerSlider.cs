// ShotPowerSlider.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShotPowerSlider : MonoBehaviour
{
    [Header("UI References")]
    public Slider powerSlider;
    [SerializeField] private RectTransform sliderAreaRect;
    public RectTransform perfectZoneIndicator;
    public RectTransform backboardZoneIndicator;

    [Header("Shot Configuration")]
    public List<ShotZone> zones;
    public ShotManager shotManager;

    // internal state
    private bool isDragging = false;
    private float finalValue = 0f;
    private bool wasInProgressLastFrame = false;

    void Awake()
    {
        ResetSlider();
    }

    void OnEnable()
    {
        ResetSlider();
    }

    private void ResetSlider()
    {
        isDragging = false;
        finalValue = 0f;
        wasInProgressLastFrame = false;
        if (powerSlider != null)
            powerSlider.value = 0f;
    }

    void Start()
    {
        powerSlider.direction = Slider.Direction.BottomToTop;
        UpdateIndicators();
    }

    void Update()
    {
        bool inProgress = shotManager.IsShotInProgress();

        // while dragging & shot not yet started, ramp up
        if (isDragging && !inProgress)
            powerSlider.value = Mathf.Clamp01(powerSlider.value + Time.deltaTime * 0.5f);

        // once shot is in flight, lock at finalValue
        if (inProgress)
            powerSlider.value = finalValue;

        // one‑time reset when the shot truly ends
        if (wasInProgressLastFrame && !inProgress)
            ResetSlider();

        wasInProgressLastFrame = inProgress;
    }

    /// <summary>Called by SwipeInputHandler on MouseDown</summary>
    public void StartDrag()
    {
        isDragging = true;
        Debug.Log("[ShotPowerSlider] StartDrag()");
    }

    /// <summary>Called by SwipeInputHandler on FireShot or by AI via TriggerShotAt()</summary>
    public void StopDrag()
    {
        // no early return: always fire
        isDragging = false;
        finalValue = powerSlider.value;
        Debug.Log($"[ShotPowerSlider] StopDrag() -> value={finalValue:F2}");
        EvaluateShot(finalValue);
    }

    /// <summary>For your AI: directly fire at a given fill %</summary>
    public void TriggerShotAt(float value)
    {
        finalValue = Mathf.Clamp01(value);
        Debug.Log($"[ShotPowerSlider] TriggerShotAt({finalValue:F2})");
        EvaluateShot(finalValue);
    }

    private void EvaluateShot(float value)
    {
        foreach (var zone in zones)
        {
            if (value >= zone.minValue && value <= zone.maxValue)
            {
                Debug.Log($"[ShotPowerSlider] EvaluateShot -> {zone.shotType}");
                shotManager.StartShot(zone.shotType);
                return;
            }
        }
        Debug.Log("[ShotPowerSlider] EvaluateShot -> LowMiss");
        shotManager.StartShot(ShotType.LowMiss);
    }

    private void UpdateIndicators()
    {
        foreach (var zone in zones)
        {
            if (zone.shotType == ShotType.Perfect && perfectZoneIndicator != null)
                SetZoneIndicator(perfectZoneIndicator, zone);
            else if (zone.shotType == ShotType.Backboard && backboardZoneIndicator != null)
                SetZoneIndicator(backboardZoneIndicator, zone);
        }
    }

    private void SetZoneIndicator(RectTransform indicator, ShotZone zone)
    {
        float h = sliderAreaRect.rect.height;
        float height = (zone.maxValue - zone.minValue) * h;
        float bottom = zone.minValue * h;
        if (zone.shotType == ShotType.Backboard)
            bottom -= 0.04f * h;
        indicator.sizeDelta = new Vector2(indicator.sizeDelta.x, height);
        indicator.anchoredPosition = new Vector2(indicator.anchoredPosition.x, bottom);
    }
}
