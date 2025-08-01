using UnityEngine;
using UnityEngine.UI;

public class VolumeOptionsToggle : MonoBehaviour
{
    [SerializeField] private CanvasGroup optionsPanel;
    [SerializeField] private Button toggleButton;

    private bool isOpen = false;

    void Awake()
    {
        // make sure it's hidden at start
        optionsPanel.alpha = 0;
        optionsPanel.interactable = false;
        optionsPanel.blocksRaycasts = false;

        toggleButton.onClick.AddListener(ToggleOptions);
    }

    private void ToggleOptions()
    {
        isOpen = !isOpen;
        optionsPanel.alpha = isOpen ? 1 : 0;
        optionsPanel.interactable = isOpen;
        optionsPanel.blocksRaycasts = isOpen;
    }
}
