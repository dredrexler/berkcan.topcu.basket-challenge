// FireballManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIFireballManager : MonoBehaviour
{
    [Header("Settings")]
    public int threshold = 3;
    public float fireballDuration = 10f;

    private int streakCount = 0;
    private bool isFireballActive = false;
    private Coroutine fireballRoutine;

    public bool IsActive => isFireballActive;

    public int ApplyMultiplier(int basePoints)
    {
        return isFireballActive ? basePoints * 2 : basePoints;
    }

    public void OnMake()
    {
        if (isFireballActive) return;

        streakCount++;
        if (streakCount >= threshold)
            StartFireball();
    }

    public void OnMiss()
    {
        streakCount = 0;
        if (isFireballActive)
            EndFireball();
    }

    private void StartFireball()
    {
        isFireballActive = true;
        if (fireballRoutine != null) StopCoroutine(fireballRoutine);
        fireballRoutine = StartCoroutine(FireballTimer());
    }

    private IEnumerator FireballTimer()
    {
        float elapsed = 0f;
        while (elapsed < fireballDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        EndFireball();
    }

    private void EndFireball()
    {
        isFireballActive = false;
        streakCount = 0;
    }
}
