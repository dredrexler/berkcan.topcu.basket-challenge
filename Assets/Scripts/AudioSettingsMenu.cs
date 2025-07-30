using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer masterMixer;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM   = "SFXVolume";

    void Start()
    {
        // Initialize slider positions from saved prefs (or defaults)
        float musicDb = PlayerPrefs.GetFloat(MUSIC_PARAM, 0f);
        float sfxDb   = PlayerPrefs.GetFloat(SFX_PARAM,   0f);
        musicSlider.value = musicDb;
        sfxSlider.value   = sfxDb;

        // apply them
        masterMixer.SetFloat(MUSIC_PARAM, musicDb);
        masterMixer.SetFloat(SFX_PARAM,   sfxDb);

        // hook events
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float db)
    {
        masterMixer.SetFloat(MUSIC_PARAM, db);
        PlayerPrefs.SetFloat(MUSIC_PARAM, db);
    }

    public void SetSFXVolume(float db)
    {
        masterMixer.SetFloat(SFX_PARAM, db);
        PlayerPrefs.SetFloat(SFX_PARAM, db);
    }
}
