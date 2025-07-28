// FireballManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireballManager : MonoBehaviour
{
    [Header("Settings")]
    public int threshold = 3;
    public float fireballDuration = 10f;

    [Header("UI")]
    public Slider barSlider;

    [SerializeField] private FireballText fireballText;
    [SerializeField] private ParticleSystem fireballBallVFXPrefab;
    // Instance of the ball effect
    private ParticleSystem ballVFXInstance;
    private int streakCount = 0;
    private bool isFireballActive = false;
    private Coroutine fireballRoutine;

    public bool IsActive => isFireballActive;
    string startMessage = "FIREBALL ACTIVE!";
    Color startColor = Color.red;
    string endMessage = "FIREBALL DEACTIVE!";
    Color endColor = Color.grey;
    public int ApplyMultiplier(int basePoints)
    {
        return isFireballActive ? basePoints * 2 : basePoints;
    }

    public void OnMake()
    {
        if (isFireballActive) return;

        streakCount++;
        barSlider.value = (float)streakCount / threshold;
        if (streakCount >= threshold)
            StartFireball();
    }

    public void OnMiss()
    {
        streakCount = 0;
        barSlider.value = 0f;
        if (isFireballActive)
            EndFireball();
    }


    private void StartFireball()
    {
        isFireballActive = true;
        fireballText.ShowFireballMessage(startMessage, startColor);
        // Spawn VFX on the ball
        SpawnBallVFX();
        if (fireballRoutine != null) StopCoroutine(fireballRoutine);
        fireballRoutine = StartCoroutine(FireballTimer());
    }

    private IEnumerator FireballTimer()
    {
        float elapsed = 0f;
        while (elapsed < fireballDuration)
        {
            elapsed += Time.deltaTime;
            barSlider.value = 1f - (elapsed / fireballDuration);
            yield return null;
        }
        EndFireball();
    }

    private void EndFireball()
    {
        fireballText.ShowFireballMessage(endMessage, endColor);
        // Stop and clean up the ball VFX
        if (ballVFXInstance != null)
        {
            ballVFXInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(ballVFXInstance.gameObject, ballVFXInstance.main.startLifetime.constantMax);
            ballVFXInstance = null;
        }
        isFireballActive = false;
        streakCount = 0;
        barSlider.value = 0f;
    }
    
    private void SpawnBallVFX()
    {
        if (fireballBallVFXPrefab == null) return;

        // Find the current ball in play
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;

        // Instantiate and parent it to the ball
        ballVFXInstance = Instantiate(
            fireballBallVFXPrefab,
            ball.transform.position,
            fireballBallVFXPrefab.transform.rotation,
            ball.transform
        );

        // Make sure it loops
        var main = ballVFXInstance.main;
        main.loop = true;
        ballVFXInstance.Play();
    }
}
