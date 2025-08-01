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

    
    public void StartDrag()
    {
        isDragging = true;
        Debug.Log("[ShotPowerSlider] StartDrag()");
    }

    
    public void StopDrag()
    {
        // no early return: always fire
        isDragging = false;
        finalValue = powerSlider.value;
        Debug.Log($"[ShotPowerSlider] StopDrag() -> value={finalValue:F2}");
        EvaluateShot(finalValue);
    }

    
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
        // zone.minValue/maxValue are in 0..1
        indicator.SetParent(sliderAreaRect, false);

        // stretch full width, but only span the vertical slice [min,max]
        indicator.anchorMin = new Vector2(0f, zone.minValue);
        indicator.anchorMax = new Vector2(1f, zone.maxValue);

        // zero‐out any offsets so corners snap exactly to the anchors
        indicator.offsetMin = Vector2.zero;
        indicator.offsetMax = Vector2.zero;
    }

}
