using UnityEngine;

public class BouncingBallMenu : MonoBehaviour
{
    public float bounceHeight = 1f;      // How high the ball goes
    public float bounceSpeed = 2f;       // How fast the ball bounces
    public float baseY = 0f;             // The Y position from which it bounces

    private Vector3 startPosition;

    void Start()
    {
        startPosition = new Vector3(transform.position.x, baseY, transform.position.z);
    }

    void Update()
    {
        float newY = baseY + Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed) * bounceHeight);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
