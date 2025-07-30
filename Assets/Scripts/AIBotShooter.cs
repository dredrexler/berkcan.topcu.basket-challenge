using UnityEngine;
using System.Collections;

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard,
    Impossible
}

public class AIBotShooter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AIBallShooter aiBallShooter;
    [SerializeField] private Transform botRootTransform;
    [SerializeField] private Transform target;
    [SerializeField] private ShotPositionManager positionManager;
    [SerializeField] private BallShooter playerBallShooter;
    [SerializeField] private AIShotManager aiShotManager;
    [SerializeField] private BackboardBonusManager bonusManager;
    [SerializeField] private Animator animator;
    [SerializeField] private AIDribbleBall aidribbleBall;

    [Header("AI Settings")]
    [Range(0f, 1f)] public float perfectShotChance = 0.3f;
    [Range(0f, 1f)] public float backboardShotChance = 0.4f;
    public float shootDelay = 2f;

    void Start()
    {
        ApplyDifficultySettings();

        // First position & attach the ball into the hand
        MoveNextToPlayer();
        aiBallShooter.AttachBallToHand();
        aidribbleBall?.StartDribble();
        animator.SetBool("AIisBouncing", true);

        StartCoroutine(ShootingLoop());
    }

    private void ApplyDifficultySettings()
    {
        switch (GameManager.Instance.SelectedDifficulty)
        {
            case AIDifficulty.Easy:
                perfectShotChance = 0.1f; backboardShotChance = 0.2f; shootDelay = 4f; break;
            case AIDifficulty.Medium:
                perfectShotChance = 0.3f; backboardShotChance = 0.4f; shootDelay = 3f; break;
            case AIDifficulty.Hard:
                perfectShotChance = 0.5f; backboardShotChance = 0.3f; shootDelay = 2.5f; break;
            case AIDifficulty.Impossible:
                perfectShotChance = 0.8f; backboardShotChance = 0.15f; shootDelay = 1.2f; break;
        }
    }

    private IEnumerator ShootingLoop()
    {
        yield return new WaitUntil(() => GameManager.Instance.GameStarted);
        while (true)
        {
            yield return new WaitUntil(() => aiShotManager.CanShoot());

            // re‑position and re‑attach before firing
            MoveNextToPlayer();
            aidribbleBall?.StartDribble();
            animator.SetBool("AIisBouncing", true);

            // choose & fire
            ShotType chosen = ChooseShot();
            aiShotManager.StartShot(chosen);

            yield return new WaitForSeconds(shootDelay);
        }
    }

    private void MoveNextToPlayer()
    {
        int idx = playerBallShooter.currentPositionIndex;
        Vector3 basePos = positionManager.GetPositionByIndex(idx);
        aiBallShooter.currentPositionIndex = idx;

        // position the bot root
        Vector3 botPos = basePos + new Vector3(1f, -2.1f, 0.5f);
        botRootTransform.position = botPos;
        botRootTransform.LookAt(target);
        var e = botRootTransform.rotation.eulerAngles;
        e.x += 20f;
        botRootTransform.rotation = Quaternion.Euler(e);

        // move the ball to where the hand would be
        Vector3 ballPos = botPos + new Vector3(0f, 2.1f, 0f);
        aiBallShooter.MoveToPosition(ballPos);
        aiBallShooter.transform.LookAt(target);
    }

    private ShotType ChooseShot()
    {
        if (bonusManager.HasActiveBonus())
            return ShotType.Backboard;

        float roll = Random.value;
        if (roll <= perfectShotChance) return ShotType.Perfect;
        if (roll <= perfectShotChance + backboardShotChance) return ShotType.Backboard;
        return ShotType.Rim;
    }
}
