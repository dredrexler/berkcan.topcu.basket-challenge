using UnityEngine;
using TMPro;

public enum BackboardBonusType
{
    None,
    Common,
    Rare,
    VeryRare
}

public class BackboardBonusManager : MonoBehaviour
{

    public Transform backboardPosition; // Position near the hoop/backboard

    public BackboardBonusType currentBonus = BackboardBonusType.None;

    public TextMeshProUGUI bonusText;
    private int remainingShots = 0; // Counts down from 2 after any shot attempt
    private int cooldownShotsRemaining = 0; // Cooldown for bonus after a shot

    [SerializeField] private FloatingTextManager floatingTextManager;

    public void TrySpawnBonus()
    {

        if (remainingShots > 0 || cooldownShotsRemaining > 0) return; // A bonus is already active

        float roll = Random.Range(0f, 100f);
        Debug.Log($"Backboard bonus roll: {roll}");
        Debug.Log($"Current bonus: {currentBonus}, Remaining shots: {remainingShots}, Cooldown: {cooldownShotsRemaining}");
        if (roll <= 15f)
            currentBonus = BackboardBonusType.VeryRare;
        else if (roll <= 25f)
            currentBonus = BackboardBonusType.Rare;
        else if (roll <= 40f)
            currentBonus = BackboardBonusType.Common;
        else
            currentBonus = BackboardBonusType.None;

        if (currentBonus != BackboardBonusType.None)
        {
            remainingShots = 3;
            cooldownShotsRemaining = 5;
            ShowBonusText();
        }
    }

    private void ShowBonusText()
    {
        if (bonusText == null) return;

        int points = GetRawBonusPoints();
        bonusText.text = $"+{points}";
        bonusText.color = currentBonus switch
        {
            BackboardBonusType.Common => Color.yellow,
            BackboardBonusType.Rare => Color.cyan,
            BackboardBonusType.VeryRare => Color.magenta,
            _ => Color.white
        };

        floatingTextManager.ShowMessage("Backboard Bonus Activated!!", Color.magenta);
        bonusText.gameObject.SetActive(true);
    }


    /// Returns the bonus points for the current shot, or 0 if no bonus.
    /// Also decrements the bonus counter (even for non-backboard shots).

    public int GetBonusPoints()
    {
        if (currentBonus == BackboardBonusType.None)
            return 0;
        
        if (remainingShots <= 0)
            ClearBonus();
        return GetRawBonusPoints();
    }

    private int GetRawBonusPoints()
    {
        return currentBonus switch
        {
            BackboardBonusType.Common => 4,
            BackboardBonusType.Rare => 6,
            BackboardBonusType.VeryRare => 8,
            _ => 0
        };
    }

    public void ClearBonus()
    {
        currentBonus = BackboardBonusType.None;
        remainingShots = 0;

        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
    }

    public void RegisterShot()
    {
        if (cooldownShotsRemaining > 0)
            cooldownShotsRemaining--;

        if (currentBonus == BackboardBonusType.None) return;

        remainingShots--;
        if (remainingShots <= 0)
            ClearBonus();
    }
    
    public bool HasActiveBonus()
    {
        return currentBonus != BackboardBonusType.None;
    }
}
