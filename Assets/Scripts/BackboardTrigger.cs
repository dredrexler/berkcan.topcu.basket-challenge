using UnityEngine;

public class BackboardTrigger : MonoBehaviour
{
    [SerializeField] private Transform hoopTarget;
    [SerializeField] private float timeToReach = 0.6f;
    [SerializeField, Range(0.1f, 1f)] private float verticalDamping = 0.8f; // NEW

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball") || !collision.collider.CompareTag("AIBall")) return;

        BallStatus status = collision.collider.GetComponent<BallStatus>();
        if (status == null || status.shotType != ShotType.Backboard || status.hasScored) return;

        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 start = rb.position;
        Vector3 end = hoopTarget.position;
        Vector3 gravity = Physics.gravity;

        Vector3 displacement = end - start;
        Vector3 velocity = (displacement - 0.5f * gravity * timeToReach * timeToReach) / timeToReach;

        // Dampen the vertical Y velocity for more natural arc
        velocity.y *= verticalDamping;

        rb.velocity = velocity;
    }
}
