// DribbleBall.cs
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
    private float handY;
    private float groundY;

    void Update()
    {
        if (!isDribbling || handTransform == null)
            return;

        // follow hand X/Z
        Vector3 pos;
        pos.x = handTransform.position.x;
        pos.z = handTransform.position.z;

        // cosine bob: at timer=0, cos(0)=1 -> hand level, then falls
        timer += Time.deltaTime * frequency * Mathf.PI * 2f;
        float t = (Mathf.Cos(timer) + 1f) * 0.5f;  // [0..1]
        pos.y = Mathf.Lerp(groundY, handY, t);

        transform.position = pos;
    }

    /// Starts the looping dribble (snaps immediately just below the hand).
    public void StartDribble()
    {
        if (handTransform == null)
        {
            Debug.LogError("DribbleBall: handTransform not set!");
            return;
        }

        // Recalculate the vertical range in case the hand has moved
        handY = handTransform.position.y;
        groundY = handY - dropDistance;

        isDribbling = true;
        timer = 0f;

        // start just below hand so it immediately falls
        transform.position = handTransform.position + Vector3.down * (dropDistance * 0.5f);
    }

    /// Stops dribbling, leaves ball where it is.
    public void StopDribble()
    {
        isDribbling = false;
    }
}
