using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShotPowerSlider : MonoBehaviour
{
    public Slider powerSlider;
    public List<ShotZone> zones; // Configurable zones

    public RectTransform perfectZoneIndicator;
    public RectTransform backboardZoneIndicator;

    private bool isDragging = false;
    private float finalValue = 0f;

    public ShotManager shotManager;

    void Start()
    {
        powerSlider.value = 0;
        powerSlider.direction = Slider.Direction.BottomToTop;
        UpdateIndicators();
    }

    void Update()
    {
        if (isDragging)
        {
            float value = Mathf.Clamp01(powerSlider.value + (Time.deltaTime * 0.5f)); // Control fill speed
            powerSlider.value = value;
        }
    }

    public void StartDrag()
    {
        isDragging = true;
        powerSlider.value = 0f;
    }

    public void StopDrag()
    {
        isDragging = false;
        finalValue = powerSlider.value;
        EvaluateShot(finalValue);
        powerSlider.value = 0;
    }

    private void EvaluateShot(float value)
    {
        foreach (var zone in zones)
        {
            if (value >= zone.minValue && value <= zone.maxValue)
            {
                shotManager.StartShot(zone.shotType);
                return;
            }
        }

        // Fallback if no zone matched
        shotManager.StartShot(ShotType.LowMiss);
    }

    private void UpdateIndicators()
    {
        foreach (var zone in zones)
        {
            if (zone.shotType == ShotType.Perfect && perfectZoneIndicator)
            {
                SetZoneIndicator(perfectZoneIndicator, zone);
            }
            else if (zone.shotType == ShotType.Backboard && backboardZoneIndicator)
            {
                SetZoneIndicator(backboardZoneIndicator, zone);
            }
        }
    }

    private void SetZoneIndicator(RectTransform indicator, ShotZone zone)
    {
        float height = ((zone.maxValue - zone.minValue) * ((RectTransform)powerSlider.fillRect).rect.height);
        float center = (zone.minValue + zone.maxValue) / 2f * ((RectTransform)powerSlider.fillRect).rect.height;
        indicator.sizeDelta = new Vector2(indicator.sizeDelta.x, height);
        indicator.anchoredPosition = new Vector2(indicator.anchoredPosition.x, center);
    }

    public ShotType GetShotTypeFromValue(float value)
    {
        foreach (var zone in zones)
        {
            if (value >= zone.minValue && value <= zone.maxValue)
                return zone.shotType;
        }

        return ShotType.LowMiss; // Optional fallback, shouldn't happen if zones cover full range
    }

    public void SetFill(float value)
    {
        powerSlider.value = value;
    }

    public void TriggerShot()
    {
        EvaluateShot(powerSlider.value);
        powerSlider.value = 0;
    }


}
