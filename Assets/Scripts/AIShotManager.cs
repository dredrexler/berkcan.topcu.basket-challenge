using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AIShotManager : MonoBehaviour
{
    [Header("AI References")]
    [SerializeField] private AIBallShooter aiBallShooter;
    [SerializeField] private Transform aiShooterTransform;
    [SerializeField] private ShotPositionManager positionManager;
    [SerializeField] private BackboardBonusManager bonusManager;
    [SerializeField] private BallShooter playerBallShooter;
    [SerializeField] private Animator animator;
    [SerializeField] private AIDribbleBall aidribbleBall;
    [SerializeField] private Button aiDebugShootButton;
    [SerializeField] private TMP_Dropdown aiShotTypeDropdown;

    private bool shotInProgress = false;
    private Vector3 pendingLaunchPos;
    private Transform aiBallTransform;
    private Vector3 aiBallOriginalScale;


    private void Awake()
    {
        // Cache the actual ball transform and its original scale
        aiBallTransform = aiBallShooter.AIballTransform;
        aiBallOriginalScale = aiBallTransform.localScale;
    }

    private void Start()
    {
        // ensure only the AI shot fires
        aiDebugShootButton.onClick.RemoveAllListeners();
        aiDebugShootButton.onClick.AddListener(() =>
        {
            var shot = (ShotType)aiShotTypeDropdown.value;
            StartShot(shot);
        });

        // initial position and dribble
        UpdatePositionToMatchPlayer();
        pendingLaunchPos = aiBallTransform.position;

        // attach ball to hand and reset scale
        aiBallShooter.AttachBallToHand();
        aiBallTransform.localScale = aiBallOriginalScale;
        aidribbleBall?.StartDribble();
        animator.SetBool("AIisBouncing", true);

    }

    public void DebugShootSelected()
    {
        // read the dropdown’s value, cast to your enum, then call the real StartShot:
        var type = (ShotType)aiShotTypeDropdown.value;
        StartShot(type);
    }


    /// Called by AIBotShooter's loop; detaches & shoots at once.

    public void StartShot(ShotType type)
    {
        if (GameManager.Instance.IsInReplay)
        return;

        if (!GameManager.Instance.GameStarted) return;
        if (GameManager.Instance.IsChangingPosition || shotInProgress)
            return;

        shotInProgress = true;

        // prepare shot: stop dribble & animation
        animator.SetBool("AIisBouncing", false);
        animator.SetTrigger("AIShoot");
        aidribbleBall?.StopDribble();

        // record where ball currently is (at hand)
        pendingLaunchPos = aiBallTransform.position;

        // detach ball and reset scale
        aiBallShooter.DetachBallFromHand();
        aiBallTransform.localScale = aiBallOriginalScale;

        // snap to recorded launch pos
        aiBallTransform.position = pendingLaunchPos;

        // actually shoot
        var status = aiBallShooter.GetComponent<BallStatus>();
        status.hitGround = false;
        status.hasScored = false;
        aiBallShooter.ShootWithOutcome(type);

        StartCoroutine(WaitForShotToComplete());
        
        
    }

    private IEnumerator WaitForShotToComplete()
    {
        
            
        var status = aiBallShooter.GetComponent<BallStatus>();
        yield return new WaitUntil(() => status.hitGround);
        yield return new WaitForSeconds(1f);

        // notify miss if needed
        if (!status.hasScored)
            FindObjectOfType<AIBasketTrigger>()?.OnAIMiss();

        // reposition and restart dribble
        UpdatePositionToMatchPlayer();

        // attach ball to hand and reset scale
        aiBallShooter.AttachBallToHand();
        aiBallTransform.localScale = aiBallOriginalScale;
       
        aidribbleBall?.StartDribble();
        animator.SetBool("AIisBouncing", true);
        

        // reset state
        status.hitGround = false;
        status.hasScored = false;
        shotInProgress = false;

        bonusManager.TrySpawnBonus();
    }

    public bool CanShoot()
    {
        return !shotInProgress;
        
    }

    private void UpdatePositionToMatchPlayer()
    {
        int idx = playerBallShooter.currentPositionIndex;
        Vector3 basePos = positionManager.GetPositionByIndex(idx);
        aiBallShooter.currentPositionIndex = idx;

        // position AI root next to player
        Vector3 botPos = basePos + new Vector3(1f, -2.1f, 0.5f);
        aiShooterTransform.position = botPos;
        aiShooterTransform.LookAt(aiBallShooter.target);
        var e = aiShooterTransform.rotation.eulerAngles;
        e.x += 20f;
        aiShooterTransform.rotation = Quaternion.Euler(e);

        // move ball at hand height
        Vector3 ballPos = botPos + new Vector3(0f, 2.1f, 0f);
        aiBallTransform.position = ballPos;
        aiBallTransform.rotation = aiBallShooter.AIhandTransform.rotation;
    }
}