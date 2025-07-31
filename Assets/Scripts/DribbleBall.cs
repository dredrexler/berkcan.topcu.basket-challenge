using UnityEngine;

[RequireComponent(typeof(Transform))]
public class DribbleBall : MonoBehaviour
{
    [Tooltip("The character's hand bone / where the ball should return to.")]
    public Transform handTransform;
    [Tooltip("How far below the hand the ball should bounce (world units)")]
    public float dropDistance = 1f;
    [Tooltip("How many bounces per second")]
    public float frequency = 2f;

    private bool isDribbling = false;
    private float timer = 0f;

    void Update()
    {
        if (!isDribbling || handTransform == null)
            return;

        // Always recompute in case hand moved
        float handY   = handTransform.position.y;
        float groundY = handY - dropDistance;

        // follow hand X/Z
        Vector3 pos = transform.position;
        pos.x = handTransform.position.x;
        pos.z = handTransform.position.z;

        // bob the Y with a sine: 0 -> groundY, 1 -> handY
        timer += Time.deltaTime * frequency * Mathf.PI * 2f;
        float t = (Mathf.Sin(timer) + 1f) * 0.5f;         
        pos.y = Mathf.Lerp(groundY, handY, t);

        transform.position = pos;
    }

    /// Starts the looping dribble (snaps immediately to hand).
    public void StartDribble()
    {
        if (handTransform == null)
        {
            Debug.LogError("DribbleBall: handTransform not set!");
            return;
        }

        timer = 0f;
        isDribbling = true;

        // Snap instantly to the hand so the first bounce is correct
        transform.position = handTransform.position;
    }

    /// Stops dribbling, leaving the ball at its current world position.
    public void StopDribble()
    {
        isDribbling = false;
    }
}
