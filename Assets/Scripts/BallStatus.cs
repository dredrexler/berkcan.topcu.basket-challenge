using UnityEngine;

public class BallStatus : MonoBehaviour
{
    public ShotType shotType;
    public bool hasScored = false;
    public bool hitGround = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            hitGround = true;
            Debug.Log("Ball hit the ground.");
        }
    }
}
