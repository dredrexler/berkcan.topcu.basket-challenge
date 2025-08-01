using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public GameObject freezeModeInfo;
    public GameObject bonusInfo;
    public GameObject clutchInfo;

    private int basketsScored = 0;
    private int perfectStreak = 0;
    private bool inFreezeMode = false;
    private bool inBonusMode = false;
    private bool inClutchTime = false;

    private void Start()
    {
        freezeModeInfo.SetActive(false);
        bonusInfo.SetActive(false);
        clutchInfo.SetActive(false);
        ShowStep1();
    }

    void ShowStep1()
    {
        GameManager.Instance.StopTimer();
        tutorialText.text = "Welcome! First, shoot 3 baskets to get started!";
        basketsScored = 0;
        perfectStreak = 0;
        inFreezeMode = inBonusMode = inClutchTime = false;
    }

    /// <summary>
    /// Called after every shot by ShotManager/BasketTrigger
    /// </summary>
    public void OnPlayerShot(ShotType shotType, bool scored)
    {
        // Step 1: Any 3 baskets scored (made, doesn't matter type)
        if (!inFreezeMode && !inBonusMode && !inClutchTime)
        {
            if (scored)
                basketsScored++;

            if (basketsScored >= 3)
                ShowFreezeStep();
            return;
        }

        // Step 2: Freeze mode, 3 perfects in a row
        if (inFreezeMode && !inBonusMode)
        {
            if (shotType == ShotType.Perfect && scored)
                perfectStreak++;
            else if (scored)
                perfectStreak = 0; // Reset if scored but not perfect

            if (perfectStreak >= 3)
                Invoke(nameof(ShowBonusStep), 1.2f);
            return;
        }

        // Step 3: Backboard bonus, must do a backboard and score
        if (inBonusMode && !inFreezeMode)
        {
            if (shotType == ShotType.Backboard && scored)
                Invoke(nameof(ShowClutchStep), 1.2f);
            return;
        }

        // Step 4: Clutch time, just wait for timer to finish
        // Optionally, track if you want
    }

    void ShowFreezeStep()
    {
        tutorialText.text = "Now let's try Freeze Mode!\nMake 3 perfect shots in a row to freeze your opponent.";
        freezeModeInfo.SetActive(true);
        inFreezeMode = true;
        inBonusMode = inClutchTime = false;
        perfectStreak = 0;
        GameManager.Instance.FreezeModeEnabled = true;
        FreezeManager.Instance.StartFreezeModePeriod();
    }

    void ShowBonusStep()
    {
        freezeModeInfo.SetActive(false);
        inFreezeMode = false;
        tutorialText.text = "Now let's check out Bonus Shots!\nDo a backboard shot to trigger a bonus!";
        bonusInfo.SetActive(true);
        inBonusMode = true;
        // Force a bonus
        var bb = FindObjectOfType<BackboardBonusManager>();
        if (bb) bb.currentBonus = BackboardBonusType.Common;
    }

    void ShowClutchStep()
    {
        bonusInfo.SetActive(false);
        inBonusMode = false;
        tutorialText.text = "Last up: Clutch Time!\nDuring the last 20 seconds, you get x2 points!";
        clutchInfo.SetActive(true);
        inClutchTime = true;
        // Activate clutch: set timer to 20 and enable clutch mode
        GameManager.Instance.GameDuration = 20f;
        GameManager.Instance.IsClutchTimeEnabled = true;
        GameManager.Instance.StartTimer();
    }

    public void OnClutchTimeDone()
    {
        clutchInfo.SetActive(false);
        inClutchTime = false;
        tutorialText.text = "Tutorial Complete!\nEnjoy the game!";
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
        Invoke(nameof(GoToMainMenu), 2.0f);
    }

    void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
