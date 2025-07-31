// ShotManager.cs
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ShotManager : MonoBehaviour
{
    [SerializeField] private BallShooter ballShooter;
    [SerializeField] private Transform ballTransform;
    [SerializeField] private Transform shooter;
    [SerializeField] private ShotPositionManager positionManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private Slider shotSlider;
    [SerializeField] private FireballManager fireballManager;
    [SerializeField] private Animator animator;
    [SerializeField] private DribbleBall dribbleBall;
    [SerializeField] private ReplayManager replayManager;
    [SerializeField] private BasketTrigger basketTrigger;

   

    private bool shotInProgress = false;
    private Vector3 ballOriginalScale;
    private Vector3 launchPosition;
    

    void Start()
    {
        Debug.Log($"[ShotManager] InCampaign = {GameManager.Instance.InCampaign}");

        ballOriginalScale = ballTransform.transform.localScale;
        SetInitialPosition();
        GameManager.Instance.SetPositionLock(false);
        animator.SetBool("IsBouncing", true);
        //ballShooter.DetachBallFromHand();
        ballShooter.AttachBallToHand();
        ballShooter.transform.localScale = ballOriginalScale;
        // ensure scale reset
        ballShooter.transform.localScale = Vector3.one;
        dribbleBall?.StartDribble();
    }

    private void SetInitialPosition()
    {
        int index;
        Vector3 pos = positionManager.GetRandomPosition(out index);
        ballShooter.currentPositionIndex = index;

        // Cache launch position for shooting
        launchPosition = pos;

        // Position shooter
        Vector3 adjusted = pos;
        adjusted.y -= 2.1f;
        shooter.position = adjusted;
        shooter.LookAt(ballShooter.target);

        Vector3 euler = shooter.rotation.eulerAngles;
        euler.x += 20f;
        shooter.rotation = Quaternion.Euler(euler);

        // Place ball at court position & parent to hand
        ballShooter.MoveToPosition(pos);
        ballShooter.transform.LookAt(ballShooter.target);
        
    }

    public void StartShot(ShotType type)
    {
        if (GameManager.Instance.IsInReplay)
        return;
        if (!GameManager.Instance.GameStarted) return;
        if (GameManager.Instance.IsChangingPosition)
        {
            Debug.LogWarning("Can't shoot: changing position");
            return;
        }
        if (!shotSlider.interactable)
        {
            Debug.LogWarning("Can't shoot: UI slider not interactable");
            return;
        }

        // Stop dribble and play shoot animation
        animator.SetBool("IsBouncing", false);
        animator.SetTrigger("Shoot");
        dribbleBall?.StopDribble();

        // Detach from hand, reset to cached launch pos, and fire
        ballShooter.DetachBallFromHand();
        ballShooter.transform.position = launchPosition;

        Debug.Log("Trying to start shot");
        BallStatus status = ballShooter.GetComponent<BallStatus>();
        status.hitGround = false;
        status.hasScored = false;
        cameraController.ZoomToHoop();

        int bonus = backboardBonusManager.GetBonusPoints();
        if (type == ShotType.Backboard && bonus > 3 && GameManager.Instance.IsReplayEnabled)
        {
            replayManager.StartRecording();
        }

        GameManager.Instance.SetPositionLock(true);
        shotSlider.interactable = false;
        shotInProgress = true;
        ballShooter.ShootWithOutcome(type);
        StartCoroutine(WaitForShotToComplete(type));
    }

    private IEnumerator WaitForShotToComplete(ShotType type)
    {
        Debug.Log("Waiting for shot to complete...");

        // Wait until the ball hits the ground
        {
            BallStatus status = ballShooter.GetComponent<BallStatus>();
            yield return new WaitUntil(() => status.hitGround);
            Debug.Log("Ball hit ground - continuing shot cycle");
            int bonus = backboardBonusManager.GetBonusPoints();
            if (type == ShotType.Backboard && bonus > 3 && status.hasScored && GameManager.Instance.IsReplayEnabled)
            {
                replayManager.StopRecordingAndPlayReplay();
                yield return new WaitForSeconds(replayManager.RecordedFrameCount * (replayManager.recordInterval / replayManager.replaySpeed) + 1f);
            }

            yield return new WaitForSeconds(2f);
            GameManager.Instance.SetPositionLock(false);
            shotSlider.interactable = true;

            if (!status.hasScored)
            {
                Debug.Log("Missed shot, moving to new position");
                int newIndex;
                Vector3 newPos = positionManager.GetRandomPosition(out newIndex);
                ballShooter.currentPositionIndex = newIndex;

                // cache launch position and move shooter
                launchPosition = newPos;
                Vector3 adjusted = newPos;
                adjusted.y -= 2.1f;
                shooter.position = adjusted;
                shooter.LookAt(ballShooter.target);

                Vector3 euler = shooter.rotation.eulerAngles;
                euler.x += 20f;
                shooter.rotation = Quaternion.Euler(euler);

                // place the ball and restart dribble
                ballShooter.MoveToPosition(newPos);
                ballShooter.transform.LookAt(ballShooter.target);
                ballShooter.AttachBallToHand();
                ballShooter.transform.localScale = ballOriginalScale;
                dribbleBall?.StartDribble();
                fireballManager.OnMiss();
            }
            else
            {
                Debug.Log("Shot scored, moving to new position");
                int newIndex;
                Vector3 newPos = positionManager.GetRandomPosition(out newIndex);
                ballShooter.currentPositionIndex = newIndex;

                // Cache new launch pos and move shooter
                launchPosition = newPos;
                Vector3 adjusted = newPos;
                adjusted.y -= 2.1f;
                shooter.position = adjusted;
                shooter.LookAt(ballShooter.target);

                Vector3 euler = shooter.rotation.eulerAngles;
                euler.x += 20f;
                shooter.rotation = Quaternion.Euler(euler);

                // Place ball at new court position & attach to hand/dribble
                ballShooter.MoveToPosition(newPos);
                ballShooter.transform.LookAt(ballShooter.target);
                ballShooter.AttachBallToHand();
                ballShooter.transform.localScale = ballOriginalScale;


                dribbleBall?.StartDribble();


            }

            backboardBonusManager.TrySpawnBonus();
            backboardBonusManager.RegisterShot();

            status.hitGround = false;
            status.hasScored = false;
            shotInProgress = false;

            cameraController.ResetToPlayerView();


            animator.SetBool("IsBouncing", true);


        }
    }
    // UI quick methods
    public void ShootPerfect()       => StartShot(ShotType.Perfect);
    public void ShootBackboard()     => StartShot(ShotType.Backboard);
    public void ShootRim()           => StartShot(ShotType.Rim);
    public void ShootLowMiss()       => StartShot(ShotType.LowMiss);
    public void ShootCloseMiss()     => StartShot(ShotType.CloseMiss);
    public void ShootBackboardMiss() => StartShot(ShotType.BackboardMiss);

    public bool IsShotInProgress() => shotInProgress;
}