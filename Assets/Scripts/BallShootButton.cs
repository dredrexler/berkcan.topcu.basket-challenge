using UnityEditor.PackageManager;
using UnityEngine;

public class BallShootButton : MonoBehaviour
{
    [SerializeField] private BallShooter ballShooter;
    

        public void ShootPerfect()
    {
        ballShooter.ShootWithOutcome(ShotType.Perfect);
    }

    public void ShootBackboard()
    {
        ballShooter.ShootWithOutcome(ShotType.Backboard);
    }

    public void ShootRim()
    {
        ballShooter.ShootWithOutcome(ShotType.Rim);
    }

    public void ShootMiss()
    {
        ballShooter.ShootWithOutcome(ShotType.LowMiss);
    }

    public void ShootCloseMiss()
    {
        ballShooter.ShootWithOutcome(ShotType.CloseMiss);
    }

    public void ShootBackboardMiss()
    {
        ballShooter.ShootWithOutcome(ShotType.BackboardMiss);
    }
}
