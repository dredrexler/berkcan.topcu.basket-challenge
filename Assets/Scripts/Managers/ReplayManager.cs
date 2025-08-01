using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class ReplayManager : MonoBehaviour
{
    public CinemachineVirtualCamera replayCam;
    public CinemachineVirtualCamera virtualCam;
    public GameObject ball;
    public float recordInterval = 0.2f;
    public float replaySpeed = 0.5f; // Slow-motion factor

    private List<Vector3> recordedPositions = new List<Vector3>();
    public int RecordedFrameCount => recordedPositions.Count; // Expose frame count

    private bool isRecording = false;

    public void StartRecording()
    {
        recordedPositions.Clear();
        isRecording = true;
        StartCoroutine(RecordBallPosition());
    }

    public void StopRecordingAndPlayReplay()
    {
        isRecording = false;
        StartCoroutine(PlayReplay());
    }

    public IEnumerator RecordBallPosition()
    {
        while (isRecording)
        {
            recordedPositions.Add(ball.transform.position);
            yield return new WaitForSeconds(recordInterval);
        }
    }

    public IEnumerator PlayReplay()
    {
        GameManager.Instance.StopTimer(); // Stop the game timer
        replayCam.Priority = 20; // Make replayCam the active camera
        virtualCam.Priority = 5; // Deactivate main camera temporarily
        ball.GetComponent<Rigidbody>().isKinematic = true; // Freeze ball physics

        foreach (Vector3 position in recordedPositions)
        {
            ball.transform.position = position;
            yield return new WaitForSeconds(recordInterval / replaySpeed);
        }

        yield return new WaitForSeconds(0.5f); // Wait briefly at end of replay

        ball.GetComponent<Rigidbody>().isKinematic = false; // Restore physics
        replayCam.Priority = 0; // Return to normal gameplay camera
        virtualCam.Priority = 10; // Reactivate main camera
        GameManager.Instance.ResumeTimer(); // Resume the game timer
    }
}
