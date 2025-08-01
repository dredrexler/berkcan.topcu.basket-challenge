// AIDribbleBall.cs
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class AIDribbleBall : MonoBehaviour
{
    [Tooltip("The character's hand bone / where the ball should return to.")]
    public Transform handTransform;
    [Tooltip("How far below the hand the ball should bounce (world units)")]
    public float dropDistance = 1f;
    [Tooltip("How many bounces per second")]
    public float frequency = 2f;

    private bool isDribbling = false;
    private float timer = 0f;
    private float handY;
    private float groundY;

    void Update()
    {

        if (!isDribbling || handTransform == null)
            return;

        // Always recompute in case hand moved
        float handY   = handTransform.position.y;
        float groundY = handY - dropDistance;

        // follow hand X/Z
        Vector3 pos;
        pos.x = handTransform.position.x;
        pos.z = handTransform.position.z;

        // cosine bob: at t=0, cos(0)=1 -> at hand height, then falls down
        timer += Time.deltaTime * frequency * Mathf.PI * 2f;
        float t = (Mathf.Cos(timer) + 1f) * 0.5f;  // [0..1]
        pos.y = Mathf.Lerp(groundY, handY, t);

        transform.position = pos;
    }

    /// Starts the looping dribble (snaps immediately at hand level).
    public void StartDribble()
    {
        if (handTransform == null)
        {
            Debug.LogError("AIDribbleBall: handTransform not set!");
            return;
        }

        // Recalculate vertical range
        handY = handTransform.position.y;
        groundY = handY - dropDistance;
        timer = 0f;                // start at top of bounce
        isDribbling = true;

        // snap to hand position to begin bounce from correct spot
        transform.position = handTransform.position;
    }

    /// Stops dribbling.
    public void StopDribble()
    {
        isDribbling = false;
    }
}
