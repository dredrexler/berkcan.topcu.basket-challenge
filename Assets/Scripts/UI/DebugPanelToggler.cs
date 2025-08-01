using UnityEngine;

public class DebugPanelToggler : MonoBehaviour
{
    public GameObject debugPanel;

    private bool isVisible = true;

    void Start()
    {
    
        debugPanel.SetActive(false);
        isVisible = false;
    }


    public void ToggleDebugPanel()
    {
        isVisible = !isVisible;
        debugPanel.SetActive(isVisible);
    }
}
