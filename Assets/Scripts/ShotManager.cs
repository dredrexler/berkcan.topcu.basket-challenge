using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
public class ShotManager : MonoBehaviour
{
    [SerializeField] private BallShooter ballShooter;
    [SerializeField] private Transform shooter; // the root transform for the shooter
    [SerializeField] private ShotPositionManager positionManager;

    [SerializeField] private bool shotInProgress = false;

    [SerializeField] private CameraController cameraController;

    [SerializeField] private BackboardBonusManager backboardBonusManager;
    [SerializeField] private Slider shotSlider;
    [SerializeField] private FireballManager fireballManager;
    void Start()
    {
        SetInitialPosition();
        GameManager.Instance.SetPositionLock(false);
    }

    private void SetInitialPosition()
    {
        int index;
        Vector3 pos = positionManager.GetRandomPosition(out index);
        ballShooter.currentPositionIndex = index;

        // Apply positioning logic
        Vector3 adjustedPos = pos;
        adjustedPos.y -= 2.1f;
        shooter.position = adjustedPos;
        shooter.LookAt(ballShooter.target);

        // Apply rotation offsets
        Vector3 euler = shooter.rotation.eulerAngles;
        euler.x += 28f;
        euler.y -= 180f;
        euler.z += 180f;
        shooter.rotation = Quaternion.Euler(euler);

        // Place ball and rotate it too
        ballShooter.MoveToPosition(pos);
        ballShooter.transform.LookAt(ballShooter.target);
    }
    public void StartShot(ShotType type)
    {
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

        Debug.Log("Trying to start shot");
        BallStatus status = ballShooter.GetComponent<BallStatus>();
        status.hitGround = false;
        status.hasScored = false;
        cameraController.ZoomToHoop();
        if (shotInProgress)
        {
            Debug.LogWarning("Can't shoot: shotInProgress still true");
            Debug.Log("Shot in progress, skipping");
            return;
        }
        Debug.Log("All clear – actually starting shot");
        // Start lock right when shot begins
        GameManager.Instance.SetPositionLock(true);
        if (shotSlider != null) shotSlider.interactable = false;
        Debug.Log("Starting new shot");

        shotInProgress = true;
        ballShooter.ShootWithOutcome(type);
        StartCoroutine(WaitForShotToComplete());
    }

    private IEnumerator WaitForShotToComplete()
    {
        BallStatus status = ballShooter.GetComponent<BallStatus>();

        // Wait until the ball hits the ground
        yield return new WaitUntil(() => status.hitGround);
        Debug.Log("Ball hit ground - continuing shot cycle");

        yield return new WaitForSeconds(2f); // Delay before unlocking
        GameManager.Instance.SetPositionLock(false);
        if (shotSlider != null) shotSlider.interactable = true;

        if (!status.hasScored)
        {
            Debug.Log("Missed shot, retrying at the same position");

            // Reset ball to the shooter's position
            Vector3 resetPos = shooter.position;
            resetPos.y += 2.1f;
            ballShooter.MoveToPosition(resetPos);
            ballShooter.transform.LookAt(ballShooter.target);
            fireballManager.OnMiss();
        }
        else
        {
            Debug.Log("Shot scored, moving to new position");

            int newIndex;
            Vector3 newPos = positionManager.GetRandomPosition(out newIndex);
            ballShooter.currentPositionIndex = newIndex;

            Vector3 adjustedPos = newPos;
            adjustedPos.y -= 2.1f;
            shooter.position = adjustedPos;
            shooter.LookAt(ballShooter.target);

            Vector3 euler = shooter.rotation.eulerAngles;
            euler.x += 28f;
            euler.y -= 180f;
            euler.z += 180f;
            shooter.rotation = Quaternion.Euler(euler);

            ballShooter.MoveToPosition(newPos);
            ballShooter.transform.LookAt(ballShooter.target);



        }

        backboardBonusManager.TrySpawnBonus();
        backboardBonusManager.RegisterShot();

        status.hitGround = false;
        status.hasScored = false;
        shotInProgress = false;

        cameraController.ResetToPlayerView();
    }

    public void ShootPerfect()
    {
        StartShot(ShotType.Perfect);
    }

    public void ShootBackboard()
    {
        StartShot(ShotType.Backboard);
    }

    public void ShootRim()
    {
        StartShot(ShotType.Rim);
    }

    public void ShootLowMiss()
    {
        StartShot(ShotType.LowMiss);
    }

    public void ShootCloseMiss()
    {
        StartShot(ShotType.CloseMiss);
    }

    public void ShootBackboardMiss()
    {
        StartShot(ShotType.BackboardMiss);
    }
    
    public bool IsShotInProgress()
    {
        return shotInProgress;
    }

}
