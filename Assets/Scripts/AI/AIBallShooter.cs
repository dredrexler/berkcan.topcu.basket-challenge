using UnityEngine;
using System.Collections.Generic;

public class AIBallShooter : MonoBehaviour
{
    public Transform target;
    public float timeToTarget = 1.2f;
    public Rigidbody rb;

    public List<ShotProfile> shotProfiles = new List<ShotProfile>();
    public int currentPositionIndex = 0;
    public Transform AIballTransform;
    public Transform AIhandTransform;


    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

    }

    public void MoveToPosition(Vector3 newPosition)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.position = newPosition;
    }

    public void ShootWithOutcome(ShotType type)
    {
        Vector3 offset = GetOffsetForShot(type, currentPositionIndex);
        Vector3 targetPos = target.position + offset;
        var status = GetComponent<BallStatus>();
        status.hasScored = false;

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
                targetPos += new Vector3(-0.12f, -0.2f, -0.2f);
                status.shotType = ShotType.LowMiss;
                break;
            case ShotType.CloseMiss:
                targetPos += Vector3.zero;
                status.shotType = ShotType.CloseMiss;
                break;
            case ShotType.BackboardMiss:
                targetPos += new Vector3(0f, 1.2f, -0.1f);
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

    private Vector3 GetOffsetForShot(ShotType type, int positionIndex)
    {
        foreach (var profile in shotProfiles)
        {
            if (profile.shotType == type && profile.positionIndex == positionIndex)
                return profile.offset;
        }

        return Vector3.zero;
    }
    
    // Call this to snap the ball into the hand
    public void AttachBallToHand()
    {
        AIballTransform.SetParent(AIhandTransform, worldPositionStays: false);
        AIballTransform.localPosition = Vector3.zero;
        AIballTransform.localRotation = Quaternion.identity;
    }
    
    // Call this to drop it back into world space
    public void DetachBallFromHand()
    {
        AIballTransform.SetParent(null, worldPositionStays: true);
    }
}
