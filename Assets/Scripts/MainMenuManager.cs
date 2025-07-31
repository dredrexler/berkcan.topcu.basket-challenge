using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


/// Handles Main Menu: time & difficulty selection, quick play and campaign launch.

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Dropdown to select game duration")]              
    [SerializeField] private TMP_Dropdown timeDropdown;
    [Tooltip("Dropdown to select difficulty level (Easy=0, Medium=1, Hard=2, Impossible=3)")]  
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [Tooltip("Button to start Campaign Mode")]
    [SerializeField] private Button campaignButton;


    [Header("Scene Names for Quick Play")]  
    [Tooltip("Scene names for each difficulty in the same order as difficultyDropdown options")]  
    [SerializeField] private string[] quickPlayScenes;
    [SerializeField] private DribbleBall dribbleBall;
    [SerializeField] private Toggle clutchToggle;
    [SerializeField] private Toggle replayToggle;
    [SerializeField] private Toggle freezemodeToggle;

    // Predefined durations matching timeDropdown options
    private readonly float[] durations = { 60f, 120f, 180f }; // 1m,2m,3m,10m

    void Start()
    {
        // Initialize dropdown callbacks
        difficultyDropdown.onValueChanged.AddListener(SetDifficultyFromDropdown);
        dribbleBall?.StartDribble();
        // Campaign button
        campaignButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartCampaign();
        });

        clutchToggle.isOn = GameManager.Instance.IsClutchTimeEnabled;
        replayToggle.isOn = GameManager.Instance.IsReplayEnabled;
        freezemodeToggle.isOn = GameManager.Instance.FreezeModeEnabled;
    }


    /// Called by Play button to start a normal game with selected settings.

    public void OnPlayPressed()
    {
        if (GameManager.Instance == null)
            return;

        // 1) Set duration
        int timeIndex = timeDropdown.value;
        // Safety clamp
        timeIndex = Mathf.Clamp(timeIndex, 0, durations.Length - 1);
        GameManager.Instance.GameDuration = durations[timeIndex];

        // 2) Set difficulty (also updates GameManager.SelectedDifficulty)
        int diffIndex = difficultyDropdown.value;
        SetDifficultyFromDropdown(diffIndex);

        // 3) Ensure not in campaign mode
        // (quick play clears any previous campaign state)
        // InCampaign is private, so reset via Exit
        GameManager.Instance.InCampaign = false;

        // 4) Set last played scene
        string sceneToLoad = "Gameplay"; // default/fallback
        if (diffIndex >= 0 && diffIndex < quickPlayScenes.Length)
            sceneToLoad = quickPlayScenes[diffIndex];
        else
            Debug.LogError("QuickPlayScenes array out of range or not set up correctly.");

        GameManager.Instance.SetLastPlayedScene(sceneToLoad);

        // 5) Set clutch time based on toggle
        SetClutchTime();
        GameManager.Instance.IsReplayEnabled = replayToggle.isOn;
        GameManager.Instance.FreezeModeEnabled = freezemodeToggle.isOn;

        // 6) Load scene
        SceneManager.LoadScene(sceneToLoad);
    }
    private void SetClutchTime()
    {
        GameManager.Instance.IsClutchTimeEnabled = clutchToggle.isOn;
    }


    /// Updates GameManager.SelectedDifficulty when dropdown changes.

    /// <param name="dropdownIndex">Index from the difficulty dropdown.</param>
    private void SetDifficultyFromDropdown(int dropdownIndex)
    {
        // Map 0→Easy, 1→Medium, 2→Hard, 3→Impossible
        AIDifficulty diff = AIDifficulty.Easy;
        switch (dropdownIndex)
        {
            case 0: diff = AIDifficulty.Easy; break;
            case 1: diff = AIDifficulty.Medium; break;
            case 2: diff = AIDifficulty.Hard; break;
            case 3: diff = AIDifficulty.Impossible; break;
            default:
                Debug.LogWarning("Unknown difficulty index " + dropdownIndex);
                break;
        }
        GameManager.Instance.SetDifficulty(diff);
    }
}
