using UnityEngine;
using System.Collections.Generic;
public enum ShotType
{
    Perfect,
    Backboard,
    Rim,
    LowMiss,
    CloseMiss,
    BackboardMiss
}

[System.Serializable]
public class ShotProfile
{
    public ShotType shotType;
    public int positionIndex;
    public Vector3 offset;
}

// Calculates and applies a velocity to shoot the ball toward a target.

public class BallShooter : MonoBehaviour
{
    public Transform target; // the basket
    public float timeToTarget = 1.2f; // seconds to reach hoop
    public Rigidbody rb;

    public List<ShotProfile> shotProfiles = new List<ShotProfile>();
    public int currentPositionIndex = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        rb.velocity = Vector3.zero; // stop any current motion
        rb.angularVelocity = Vector3.zero; // stop any rotation
        rb.useGravity = false; // disable gravity while moving to position
        rb.isKinematic = true; // make the Rigidbody kinematic to move it directly
        transform.position = newPosition; // move to the new position
    }

    public void ShootToTarget()
    {
        Vector3 gravity = Physics.gravity;
        Vector3 displacement = target.position - transform.position;

        Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        rb.velocity = velocity;
    }

    private Vector3 GetOffsetForShot(ShotType type, int positionIndex)
    {
        foreach (var profile in shotProfiles)
        {
            if (profile.shotType == type && profile.positionIndex == positionIndex)
            {
                return profile.offset;
            }
        }

        Debug.LogWarning($"No offset found for {type} at position {positionIndex}, using Vector3.zero");
        return Vector3.zero;
    }
    public ShotType CurrentShotType { get; private set; }

    public void ShootWithOutcome(ShotType type)
    {
        Vector3 offset = GetOffsetForShot(type, currentPositionIndex);
        Vector3 targetPos = target.position + offset;
        var status = GetComponent<BallStatus>();
        status.hasScored = false; // reset scoring status

        switch (type)
        {
            case ShotType.Perfect:
                targetPos += new Vector3(-0.12f, 0.12f, 0f);
                status.shotType = ShotType.Perfect;
                break;

            case ShotType.Backboard:
                targetPos += new Vector3(-1.8f, 1f, -0.25f);
                status.shotType = ShotType.Backboard;
                break;


            case ShotType.Rim:
                targetPos += new Vector3(-0.2f, -0.1f, 0f);
                status.shotType = ShotType.Rim;
                break;

            case ShotType.LowMiss:
                targetPos += new Vector3(-0.12f, -0.2f, -0.2f); // clear miss
                status.shotType = ShotType.LowMiss;
                break;

            case ShotType.CloseMiss:
                targetPos += new Vector3(0f, 0f, 0f); // clear miss
                status.shotType = ShotType.CloseMiss;
                break;

            case ShotType.BackboardMiss:
                targetPos += new Vector3(0f, 1.2f, -0.1f); // clear miss
                status.shotType = ShotType.BackboardMiss;
                break;
        }

        Vector3 gravity = Physics.gravity;
        Vector3 displacement = targetPos - transform.position;

        Vector3 velocity = (displacement - 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = velocity;
    }

}
