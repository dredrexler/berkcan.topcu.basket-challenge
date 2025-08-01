using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootManager : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
