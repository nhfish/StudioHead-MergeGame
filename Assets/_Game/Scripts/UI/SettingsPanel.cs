using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public Button toggleButton;
    public Button closeButton;

    private void Awake()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(Toggle);
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Close()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void Toggle()
    {
        if (panelRoot != null)
            panelRoot.SetActive(!panelRoot.activeSelf);
    }
}
