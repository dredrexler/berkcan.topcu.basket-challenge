using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BasketTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private FloatingTextManager floatingTextManager;
    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private FireballManager fireballManager;

    [Header("VFX (Scene Instances)")]
    [SerializeField] private Transform vfxSpawnPoint;
    [SerializeField] private ParticleSystem perfectVFXPrefab;
    [SerializeField] private ParticleSystem rimVFXPrefab;
    [SerializeField] private ParticleSystem backboardVFXPrefab;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip perfectSfx;
    [SerializeField] private AudioClip rimSfx;
    [SerializeField] private AudioClip backboardSfx;
    [SerializeField] private AudioClip backboardBonusSfx;

    private ParticleSystem perfectVFX;
    private ParticleSystem rimVFX;
    private ParticleSystem backboardVFX;

    void Awake()
    {
        // instantiate & parent once
        perfectVFX   = Instantiate(perfectVFXPrefab,   vfxSpawnPoint.position, perfectVFXPrefab.transform.rotation,   vfxSpawnPoint);
        rimVFX       = Instantiate(rimVFXPrefab,       vfxSpawnPoint.position, rimVFXPrefab.transform.rotation,       vfxSpawnPoint);
        backboardVFX = Instantiate(backboardVFXPrefab, vfxSpawnPoint.position, backboardVFXPrefab.transform.rotation, vfxSpawnPoint);

        // prevent auto‑play/destruct
        perfectVFX.Stop(true,  ParticleSystemStopBehavior.StopEmittingAndClear);
        rimVFX.Stop(true,      ParticleSystemStopBehavior.StopEmittingAndClear);
        backboardVFX.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.GameStarted) return;
        if (!other.CompareTag("Ball")) return;
        var status = other.GetComponent<BallStatus>();
        if (status == null || status.hasScored) return;

        Debug.Log("Ball entered the basket!");

        // 1) Determine base points, title text, color & VFX
        int    basePoints = 0;
        string title      = "";
        Color  msgColor   = Color.white;
        ParticleSystem toPlay = null;

        switch (status.shotType)
        {
            case ShotType.Perfect:
                basePoints = 3;
                title      = "Perfect Shot!";
                audioSource?.PlayOneShot(perfectSfx);
                msgColor   = Color.green;
                toPlay     = perfectVFX;
                break;

            case ShotType.Rim:
                basePoints = 2;
                title      = "Rim Shot!";
                audioSource?.PlayOneShot(rimSfx);
                msgColor   = Color.grey;
                toPlay     = rimVFX;
                break;

            case ShotType.Backboard:
                int bonus = backboardBonusManager.GetBonusPoints();
                if (bonus > 0)
                {
                    basePoints = bonus;
                    title      = "Backboard Bonus!";
                    audioSource?.PlayOneShot(backboardBonusSfx);
                    msgColor   = Color.magenta;  // purple
                    toPlay     = backboardVFX;
                }
                else
                {
                    basePoints = 2;
                    title      = "Backboard Shot!";
                    audioSource?.PlayOneShot(backboardSfx);
                    msgColor   = Color.white;
                    toPlay     = rimVFX;
                }
                break;
        }

        // 2) Apply fireball multiplier
        int totalPoints = fireballManager.ApplyMultiplier(basePoints);

        // 3) Award score
        GameManager.Instance.AddScore(totalPoints);

        // 4) Show floating text with actual (possibly doubled) points
        string message = $"{title}\n+{totalPoints} Points!";
        Color finalColor = fireballManager.IsActive ? Color.red : msgColor;
        floatingTextManager.ShowMessage(message, finalColor);

        // 5) Play VFX & advance fireball progress
        PlayVFX(toPlay);
        fireballManager.OnMake();

        // 6) Update UI and mark scored
        UpdateScoreUI();
        status.hasScored = true;
    }

    private void PlayVFX(ParticleSystem fx)
    {
        if (fx == null) return;
        fx.transform.position = vfxSpawnPoint.position;
        fx.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        fx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        fx.Play();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{GameManager.Instance.TotalScore}";
    }
}
