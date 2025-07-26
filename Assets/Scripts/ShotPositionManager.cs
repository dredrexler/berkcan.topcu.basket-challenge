using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPositionManager : MonoBehaviour
{
    [SerializeField] private Transform[] shotPositions; // Array of shot positions

    
    public Vector3 GetRandomPosition(out int index)
    {
        index = Random.Range(0, shotPositions.Length);
        return shotPositions[index].position;
    }
    
    
    public Vector3 GetPositionByIndex(int index)
    {
        index = Mathf.Clamp(index, 0, shotPositions.Length - 1); // ensure it's within bounds
        return shotPositions[index].position;
    }

    public int GetPositionCount()
    {
        return shotPositions.Length;
    }
    
}
