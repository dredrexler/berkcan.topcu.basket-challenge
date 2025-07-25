using UnityEngine;

// Detects if a Backboard Shot hits the backboard and triggers score.

public class BackboardTrigger : MonoBehaviour
{
    [SerializeField] private Transform hoopTarget; // Point above/below the basket ring
    [SerializeField] private float timeToReach = 0.6f; // time to reach basket from backboard

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball")) return;

        BallStatus status = collision.collider.GetComponent<BallStatus>();
        if (status == null || status.shotType != ShotType.Backboard || status.hasScored) return;

        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Calculate required velocity to reach the basket
        Vector3 start = rb.position;
        Vector3 end = hoopTarget.position;
        Vector3 gravity = Physics.gravity;

        Vector3 displacement = end - start;
        Vector3 velocity = (displacement - 0.5f * gravity * timeToReach * timeToReach) / timeToReach;

        // Apply velocity directly
        rb.velocity = velocity;

        
    }

}
