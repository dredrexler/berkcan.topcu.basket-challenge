using UnityEngine;
using System.Collections;

public class FreezeManager : MonoBehaviour
{
    public static FreezeManager Instance;
    [Header("Runtime Debug")]
    [SerializeField]
    private bool _isFreezeModeActive = false;
    public bool IsFreezeModeActive { get => _isFreezeModeActive; private set => _isFreezeModeActive = value; }
    [SerializeField]
    private bool _isPlayerFrozen = false;
    public bool IsPlayerFrozen { get => _isPlayerFrozen; private set => _isPlayerFrozen = value; }
    [SerializeField]
    private bool _isAIFrozen = false;
    public bool IsAIFrozen { get => _isAIFrozen; private set => _isAIFrozen = value; }

    [SerializeField]
    private int playerPerfectStreak = 0;
    [SerializeField]
    private int aiPerfectStreak = 0;
    [SerializeField]
    private bool freezeUsedThisPeriod = false;

    [SerializeField] private FloatingTextManager floatingTextManager;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private DribbleBall dribbleBall;
    [SerializeField] private ShotManager shotManager;
    [SerializeField] private AIShotManager aiShotManager;

    private int freezeCooldownShots = 0; // Shots to wait before Freeze Mode can re-activate
    public FreezeModeUI freezeModeUI;


    private void Awake()
    {
        Instance = this;
    }

    /// Call after every shot (like TrySpawnBonus). 30% chance to activate Freeze Mode (unless already active or on cooldown).

    public void TryActivateFreezeMode()
    {
        if (IsFreezeModeActive || freezeCooldownShots > 0)
            return;

        float roll = Random.Range(0f, 100f);
        Debug.Log($"Freeze Mode Roll: {roll}");
        if (roll <= 50f)
        {
            StartFreezeModePeriod();
        }
    }


    /// Call after every shot. Decrements cooldown.

    public void RegisterShot()
    {
        if (freezeCooldownShots > 0)
            freezeCooldownShots--;
    }

    public void StartFreezeModePeriod()
    {
        Debug.Log("Freeze Mode Activated!");
        IsFreezeModeActive = true;
        freezeUsedThisPeriod = false;
        playerPerfectStreak = 0;
        aiPerfectStreak = 0;
        // Show "Freeze Mode Active!" UI
        if (floatingTextManager != null)
            floatingTextManager.ShowMessage("Freeze Mode Active!", Color.cyan);
        freezeModeUI.ActivateFreezeSlider(25f);
        StartCoroutine(FreezeModeDuration());
    }

    private IEnumerator FreezeModeDuration()
    {
        yield return new WaitForSeconds(25f);
        IsFreezeModeActive = false;
        freezeUsedThisPeriod = false;
        playerPerfectStreak = 0;
        aiPerfectStreak = 0;
        freezeCooldownShots = 5; // After period, wait 5 shots before it can re-activate
        // Show "Freeze Mode Active!" UI
        if (floatingTextManager != null)
            floatingTextManager.ShowMessage("Freeze Mode Deactive!", Color.white);
        freezeModeUI.HideFreezeSlider();
    }


    /// Call when a perfect shot is scored. 
    /// isPlayer = true if player, false if AI.

    public void RegisterPerfectShot(bool isPlayer)
    {
        if (!IsFreezeModeActive || freezeUsedThisPeriod) return;

        if (isPlayer)
        {
            playerPerfectStreak++;
            aiPerfectStreak = 0;
            if (playerPerfectStreak == 3)
            {
                StartCoroutine(FreezeAI());
            }
        }
        else
        {
            aiPerfectStreak++;
            playerPerfectStreak = 0;
            if (aiPerfectStreak == 1)
            {
                StartCoroutine(FreezePlayer());
            }
        }
    }

    private IEnumerator FreezePlayer()
    {
        IsPlayerFrozen = true;
        freezeUsedThisPeriod = true;
        shotManager.SetFrozen(true);
        if (floatingTextManager != null)
            floatingTextManager.ShowMessage("You Are Frozen!", Color.blue);
        yield return new WaitForSeconds(10f);
        IsPlayerFrozen = false;
        shotManager.SetFrozen(false);

    }

    private IEnumerator FreezeAI()
    {
        IsAIFrozen = true;
        freezeUsedThisPeriod = true;
        aiShotManager.SetFrozen(true);
        if (floatingTextManager != null)
            floatingTextManager.ShowMessage("AI Is Frozen!", Color.blue);
        yield return new WaitForSeconds(10f);
        IsAIFrozen = false;
        aiShotManager.SetFrozen(false);
    }

    // Use these to check in your ShotManager or input code
    public bool CanPlayerShoot() => !IsPlayerFrozen && !GameManager.Instance.IsChangingPosition;
    public bool CanAIShoot() => !IsAIFrozen && !GameManager.Instance.IsChangingPosition;
    public void ResetPlayerStreak() { playerPerfectStreak = 0; }
    public void ResetAIStreak() { aiPerfectStreak = 0; }
    public int PlayerPerfectStreak => playerPerfectStreak;
    public int AIPerfectStreak => aiPerfectStreak;
    public bool FreezeUsedThisPeriod => freezeUsedThisPeriod;
}
