using UnityEngine;

public class PositionTester : MonoBehaviour
{
    public ShotPositionManager positionManager;
    public Transform shooter;
    public BallShooter ballShooter;
    
    private int currentIndex = 0;

    public void NextPosition()
    {
        currentIndex++;
        if (currentIndex >= positionManager.GetPositionCount())
            currentIndex = 0;

        Vector3 pos = positionManager.GetPositionByIndex(currentIndex);
        shooter.position = pos;
        Vector3 adjustedPos = pos;
        adjustedPos.y -= 2.1f; // your offset
        shooter.position = adjustedPos;

        shooter.LookAt(ballShooter.target);
        Vector3 euler = shooter.rotation.eulerAngles;
        euler.x += 20f;
        
        shooter.rotation = Quaternion.Euler(euler);

        ballShooter.MoveToPosition(pos);
        ballShooter.currentPositionIndex = currentIndex;
        ballShooter.transform.LookAt(ballShooter.target);
    }
}
