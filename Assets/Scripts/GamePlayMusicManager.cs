using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GamePlayMusicManager : MonoBehaviour
{
    private AudioSource _src;

    void Awake() {
        _src = GetComponent<AudioSource>();
    }

    void Start() {
        float duration = GameManager.Instance.GameDuration;
        if (_src.clip == null) return;

        _src.Play();
        StartCoroutine(StopMusicAfter(duration));
    }

    private IEnumerator StopMusicAfter(float secs) {
        yield return new WaitForSeconds(secs);
        _src.Stop();
    }
}
