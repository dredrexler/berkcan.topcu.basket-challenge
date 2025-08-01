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

    [Header("Ball VFX")]
    [SerializeField] private ParticleSystem fireballBallVFXPrefab;
    [SerializeField] private Vector3 vfxLocalOffset = Vector3.zero;

    [Header("Audio")]
    [Tooltip("Looping sound to play during fireball mode")]
    public AudioSource loopAudioSource;

    private int streakCount = 0;
    private bool isFireballActive = false;
    private Coroutine fireballRoutine;
    private ParticleSystem ballVFXInstance;

    public bool IsActive => isFireballActive;

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
        fireballText.ShowFireballMessage("FIREBALL ACTIVE!", Color.red);

        // play looped audio
        if (loopAudioSource != null)
        {
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }

        // spawn vfx
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
        fireballText.ShowFireballMessage("FIREBALL ENDED!", Color.white);

        // stop looped audio
        if (loopAudioSource != null)
            loopAudioSource.Stop();

        // stop and cleanup vfx
        if (ballVFXInstance != null)
        {
            ballVFXInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Destroy(ballVFXInstance.gameObject, ballVFXInstance.main.startLifetime.constantMax);
            ballVFXInstance = null;
        }
        // Stop the fireball timer if it's running
        if (fireballRoutine != null)
        {
            StopCoroutine(fireballRoutine);
            fireballRoutine = null;
        }

        isFireballActive = false;
        streakCount = 0;
        barSlider.value = 0f;
    }

    private void SpawnBallVFX()
    {
        if (fireballBallVFXPrefab == null) return;

        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball == null) return;

        ballVFXInstance = Instantiate(
            fireballBallVFXPrefab,
            ball.transform.position,
            fireballBallVFXPrefab.transform.rotation,
            ball.transform
        );
        ballVFXInstance.transform.localPosition = vfxLocalOffset;

        var main = ballVFXInstance.main;
        main.loop = true;
        ballVFXInstance.Play();
    }
}
