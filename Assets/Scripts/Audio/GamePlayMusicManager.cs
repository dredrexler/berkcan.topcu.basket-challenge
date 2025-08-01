using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GamePlayMusicManager : MonoBehaviour
{
    private AudioSource _src;
    private bool isPlaying = false;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        // Optional: Don’t destroy on load if you want it to persist across gameplay scenes
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Subscribe to game start and scene change events
        GameManager.Instance.OnGameStart += PlayMusic;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void PlayMusic()
    {
        if (_src.clip == null) return;
        if (isPlaying) return;

        _src.Play();
        isPlaying = true;
    }

    // Call this when the game actually ends, or when the scene unloads
    private void OnSceneUnloaded(Scene scene)
    {
        StopMusic();
    }

    public void StopMusic()
    {
        if (_src.isPlaying)
        {
            _src.Stop();
            isPlaying = false;
        }
    }

    private void OnDestroy()
    {
        // Clean up
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStart -= PlayMusic;

        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
