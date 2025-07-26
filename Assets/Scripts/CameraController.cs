using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera thirdPersonCam;

    [Header("Offsets")]
    public Vector3 normalOffset = new Vector3(0, 2.5f, -6f);     // Default behind player
    public Vector3 shotOffset = new Vector3(0, 4f, -3f);         // During shot (closer to hoop)
    public float transitionDuration = 0.7f;

    private CinemachineFramingTransposer transposer;
    private Coroutine transitionRoutine;

    private void Awake()
    {
        transposer = thirdPersonCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_TrackedObjectOffset = normalOffset;
    }

    public void ZoomToHoop()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionOffset(shotOffset));
    }

    public void ResetToPlayerView()
    {
        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(TransitionOffset(normalOffset));
    }

    private IEnumerator TransitionOffset(Vector3 targetOffset)
    {
        Vector3 startOffset = transposer.m_TrackedObjectOffset;
        float t = 0f;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            transposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, targetOffset, t / transitionDuration);
            yield return null;
        }

        transposer.m_TrackedObjectOffset = targetOffset;
    }
}
