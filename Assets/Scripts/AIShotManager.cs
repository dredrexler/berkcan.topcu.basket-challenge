using UnityEngine;
using System.Collections;

public class AIShotManager : MonoBehaviour
{
    [SerializeField] private AIBallShooter aiBallShooter;
    [SerializeField] private Transform aiShooterTransform;
    [SerializeField] private ShotPositionManager positionManager;
    [SerializeField] private BackboardBonusManager bonusManager;
    [SerializeField] private Transform playerTransform;           // Player root
    [SerializeField] private BallShooter playerBallShooter;       // To get currentPositionIndex from player

    private bool shotInProgress = false;

    private void Start()
    {
        UpdatePositionToMatchPlayer();
    }

    


    public void StartShot(ShotType type)
    {
        if (GameManager.Instance.IsChangingPosition)
            return;
        if (shotInProgress)
            return;

        UpdatePositionToMatchPlayer(); // <-- Move AI into place before shooting

        shotInProgress = true;

        BallStatus status = aiBallShooter.GetComponent<BallStatus>();
        status.hitGround = false;
        status.hasScored = false;

        aiBallShooter.ShootWithOutcome(type);
        StartCoroutine(WaitForShotToComplete());
    }

    private IEnumerator WaitForShotToComplete()
    {
        BallStatus status = aiBallShooter.GetComponent<BallStatus>();

        yield return new WaitUntil(() => status.hitGround);
        yield return new WaitForSeconds(1f);

        UpdatePositionToMatchPlayer();

        if (!status.hasScored)
        {
            Vector3 resetPos = aiShooterTransform.position;
            resetPos.y += 2.1f;
            aiBallShooter.MoveToPosition(resetPos);
            aiBallShooter.transform.LookAt(aiBallShooter.target);
        }

        bonusManager.TrySpawnBonus();
        //bonusManager.RegisterShot();

        status.hitGround = false;
        status.hasScored = false;
        shotInProgress = false;
    }

    public bool CanShoot()
    {
        return !shotInProgress;
    }

    public Vector3 GetShotPosition(int index)
    {
        return positionManager.GetPositionByIndex(index);
    }

    private void UpdatePositionToMatchPlayer()
    {
        int index = playerBallShooter.currentPositionIndex;
        Vector3 pos = positionManager.GetPositionByIndex(index);
        aiBallShooter.currentPositionIndex = index;

        // Position AI bot beside the player
        Vector3 botPosition = pos + new Vector3(1f, 0f, 0f);
        botPosition.y -= 2.1f;
        aiShooterTransform.position = botPosition;
        aiShooterTransform.LookAt(aiBallShooter.target);
        
        Vector3 euler = aiShooterTransform.rotation.eulerAngles;
        euler.x += 28f;
        euler.y -= 180f;
        euler.z += 180f;
        aiShooterTransform.rotation = Quaternion.Euler(euler);
        
        // Match ball to same offset position
        Vector3 ballSpawnPos = pos + new Vector3(1f, 0, 0f); // x offset + height
        aiBallShooter.MoveToPosition(ballSpawnPos);
        aiBallShooter.transform.LookAt(aiBallShooter.target);
    }

}
