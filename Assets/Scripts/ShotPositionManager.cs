using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPositionManager : MonoBehaviour
{
    [SerializeField] private Transform[] shotPositions;
    private int lastIndex = -1;  // remember last used

    /// Returns a random position (and its index) different from the previous call.
    
    public Vector3 GetRandomPosition(out int index)
    {
        int count = shotPositions.Length;
        if (count == 0)
        {
            index = -1;
            return Vector3.zero;
        }
        if (count == 1)
        {
            index = 0;
            return shotPositions[0].position;
        }

        int newIndex;
        // pick until it differs from lastIndex
        do
        {
            newIndex = Random.Range(0, count);
        } while (newIndex == lastIndex);

        lastIndex = newIndex;
        index = newIndex;
        return shotPositions[newIndex].position;
    }


    /// For direct lookups (sequential or AI logic).

    public Vector3 GetPositionByIndex(int index)
    {
        index = Mathf.Clamp(index, 0, shotPositions.Length - 1);
        return shotPositions[index].position;
    }


    /// Number of available shot positions.

    public int GetPositionCount() => shotPositions.Length;
}
