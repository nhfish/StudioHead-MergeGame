using UnityEngine;
using UnityEngine.UI;

public class SaveIndicator : MonoBehaviour
{
    [Header("Animation")]
    public Image indicatorImage;
    public float rotationSpeed = 90f; // degrees per second
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.5f);

    private bool isActive = false;

    void Start()
    {
        if (indicatorImage == null)
            indicatorImage = GetComponent<Image>();
        
        SetActive(false);
    }

    void Update()
    {
        if (isActive && indicatorImage != null)
        {
            // Rotate the indicator
            indicatorImage.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        
        if (indicatorImage != null)
        {
            indicatorImage.color = active ? activeColor : inactiveColor;
            indicatorImage.enabled = active;
        }
        
        gameObject.SetActive(active);
    }
} 