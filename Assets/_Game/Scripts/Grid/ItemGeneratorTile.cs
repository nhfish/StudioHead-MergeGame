using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ItemGeneratorTile : MonoBehaviour
{
    [Header("Config")]
    public DepartmentType departmentType;
    public int maxCharges = 5;
    public float cooldownTime = 30f;

    [Header("UI References")]
    public Button spawnButton;
    public TextMeshProUGUI chargeLabel;
    public Image cooldownOverlay;

    private int currentCharges;
    private bool isOnCooldown;

    private void Start()
    {
        currentCharges = maxCharges;
        UpdateUI();
        spawnButton.onClick.AddListener(OnGenerateClick);
    }

    private void OnGenerateClick()
    {
        if (isOnCooldown || currentCharges <= 0)
        {
            Debug.Log(" Generator on cooldown or out of charges.");
            return;
        }

        var item = DepartmentItemFactory.Create(departmentType);
        GridItemSpawner.Instance.SpawnItemNear(this.transform.position, item);

        currentCharges--;
        UpdateUI();

        if (currentCharges <= 0)
            StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        float timer = cooldownTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cooldownOverlay.fillAmount = timer / cooldownTime;
            yield return null;
        }

        isOnCooldown = false;
        currentCharges = maxCharges;
        cooldownOverlay.fillAmount = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        chargeLabel.text = $"{currentCharges}/{maxCharges}";
        spawnButton.interactable = !isOnCooldown && currentCharges > 0;
    }
}
