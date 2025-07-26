using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ShotManager : MonoBehaviour
{
    [SerializeField] private BallShooter ballShooter;
    [SerializeField] private Transform shooter; // the root transform for the shooter
    [SerializeField] private ShotPositionManager positionManager;

    [SerializeField] private bool shotInProgress = false;
    
    [SerializeField] private CameraController cameraController;

    
    public void StartShot(ShotType type)
    {
        Debug.Log("Trying to start shot");
        BallStatus status = ballShooter.GetComponent<BallStatus>();
        status.hitGround = false;
        status.hasScored = false;
        cameraController.ZoomToHoop();
        if (shotInProgress)
        {

            Debug.Log("Shot in progress, skipping");
            return;
        }
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
        yield return new WaitForSeconds(1f); // wait a moment after it hits
        
            int newIndex;
            Vector3 newPos = positionManager.GetRandomPosition(out newIndex);
            ballShooter.currentPositionIndex = newIndex; // update the current position index
            shooter.position = newPos;
            Vector3 adjustedPos = newPos;
            adjustedPos.y -= 2.1f;
            //adjustedPos.x += 2f; // adjust z to avoid clipping through the ground
            shooter.position = adjustedPos;
            shooter.LookAt(ballShooter.target);
            // Get current rotation after LookAt
            Vector3 euler = shooter.rotation.eulerAngles;

            // Apply desired offsets
            euler.x += 28f; // tilt up slightly
            euler.y -= 180f; // turn around
            euler.z += 180f; // flip

            // Apply the new rotation
            shooter.rotation = Quaternion.Euler(euler);
            ballShooter.MoveToPosition(newPos);
            ballShooter.transform.LookAt(ballShooter.target);
        
        
        if (!status.hasScored)
        {
            Debug.Log("Missed shot, retrying at the same position");

            // Reset ball to the shooter's position
            Vector3 resetPos = shooter.position;
            resetPos.y += 2.1f; // reverse the offset you applied earlier
            ballShooter.MoveToPosition(resetPos);

            // Ensure the ball is facing the hoop again
            ballShooter.transform.LookAt(ballShooter.target);
        }
        
        Debug.Log("Shot completed, moving to new position");

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

}
