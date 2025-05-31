using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalentCardUI : MonoBehaviour
{
    [Header("UI References")]
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI usesText;
    public Button selectButton;

    private TalentCard card;

    public void Initialize(TalentCard talentCard, System.Action<TalentCard> onSelected)
    {
        card = talentCard;
        nameText.text = card.baseData.talentName;
        portraitImage.sprite = card.baseData.portrait;
        usesText.text = $"{card.UsesRemaining}/{card.baseData.MaxUses}";

        selectButton.interactable = card.IsUsable;
        selectButton.onClick.AddListener(() => onSelected.Invoke(card));
    }
}
