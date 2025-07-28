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
    public AIBallShooter aiBallShooter;
    public Transform botRootTransform;
    public Transform playerTransform;
    public Transform target;

    public AIShotManager aiShotManager;
    public BackboardBonusManager bonusManager;

    [Range(0f, 1f)] public float perfectShotChance = 0.3f;
    [Range(0f, 1f)] public float backboardShotChance = 0.4f;

    public float shootDelay = 2f;

    private void Start()
    {
        ApplyDifficultySettings();
        StartCoroutine(ShootingLoop());
    }

    private void ApplyDifficultySettings()
    {
        AIDifficulty difficulty = GameManager.Instance.SelectedDifficulty;

        switch (difficulty)
        {
            case AIDifficulty.Easy:
                perfectShotChance = 0.1f;
                backboardShotChance = 0.2f;
                shootDelay = 4f;
                break;
            case AIDifficulty.Medium:
                perfectShotChance = 0.3f;
                backboardShotChance = 0.4f;
                shootDelay = 3.0f;
                break;
            case AIDifficulty.Hard:
                perfectShotChance = 0.5f;
                backboardShotChance = 0.3f;
                shootDelay = 2.5f;
                break;
            case AIDifficulty.Impossible:
                perfectShotChance = 0.8f;
                backboardShotChance = 0.15f;
                shootDelay = 1.2f;
                break;
        }
    }

    private IEnumerator ShootingLoop()
    {
        //yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitUntil(() => aiShotManager.CanShoot());

            MoveNextToPlayer();

            ShotType selectedShot = ChooseShot();
            aiShotManager.StartShot(selectedShot);

            yield return new WaitForSeconds(shootDelay);
        }
    }

    private void MoveNextToPlayer()
    {
        int currentIndex = aiBallShooter.currentPositionIndex;
        Vector3 basePosition = aiShotManager.GetShotPosition(currentIndex);

        Vector3 newPosition = basePosition + new Vector3(1f, 0f, 0.5f); // Side of player
        newPosition.y -= 2.1f;
        botRootTransform.position = newPosition;

        botRootTransform.LookAt(target);

        Vector3 euler = botRootTransform.rotation.eulerAngles;
        euler.x += 28f;
        euler.y -= 180f;
        euler.z += 180f;
        botRootTransform.rotation = Quaternion.Euler(euler);

        aiBallShooter.MoveToPosition(basePosition);
        aiBallShooter.transform.LookAt(target);
    }


    private ShotType ChooseShot()
    {
        if (bonusManager.HasActiveBonus())
            return ShotType.Backboard;

        float roll = Random.Range(0f, 1f);

        if (roll <= perfectShotChance)
            return ShotType.Perfect;

        if (roll <= perfectShotChance + backboardShotChance)
            return ShotType.Backboard;

        return ShotType.Rim;
    }
}
