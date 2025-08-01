using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameplayScene()
    {
        
        SceneManager.LoadScene("Gameplay");
    }

    public void LoadRewardScene()
    {
        SceneManager.LoadScene("Reward");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
