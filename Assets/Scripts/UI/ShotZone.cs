using UnityEngine;

[System.Serializable]
public class ShotZone
{
    public string name;
    public ShotType shotType;
    [Range(0f, 1f)] public float minValue;
    [Range(0f, 1f)] public float maxValue;
}
