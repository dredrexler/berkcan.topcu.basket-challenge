using UnityEngine;
public enum ShotType
    {
        Perfect,
        Backboard,
        Rim,
        Miss
    }

// Calculates and applies a velocity to shoot the ball toward a target.

public class BallShooter : MonoBehaviour
{
    public Transform target; // the basket
    public float timeToTarget = 1.2f; // seconds to reach hoop
    public Rigidbody rb;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ShootToTarget()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 displacement = target.position - transform.position;

        Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        rb.velocity = velocity;
    }

    public ShotType CurrentShotType { get; private set; }
    
    public void ShootWithOutcome(ShotType type)
    {
        Vector3 targetPos = target.position;
        var status = GetComponent<BallStatus>();
        status.hasScored = false; // reset scoring status

        switch (type)
        {
            case ShotType.Perfect:
                targetPos += new Vector3(0f, 0.4f, -0.25f);
                status.shotType = ShotType.Perfect;
                break;

            case ShotType.Backboard:
                targetPos += new Vector3(-0.2f, 1.2f, -0.25f);
                status.shotType = ShotType.Backboard;
                break;


            case ShotType.Rim:
                targetPos += new Vector3(0f, 0.25f, -0.25f);
                status.shotType = ShotType.Rim;
                break;

            case ShotType.Miss:
                targetPos += new Vector3(0.5f, 0.2f, 0.2f); // clear miss
                status.shotType = ShotType.Miss;
                break;
        }

        Vector3 gravity = Physics.gravity;
        Vector3 displacement = targetPos - transform.position;

        Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        rb.velocity = velocity;
    }

}
