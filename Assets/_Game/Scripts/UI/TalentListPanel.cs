using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TalentListPanel : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public TalentInventory inventory;

    public void PopulateTalentCards(System.Action<TalentCard> onTalentSelected)
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject); // Clear old cards

        List<TalentCard> available = inventory.GetAvailableCards();

        foreach (var card in available)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardParent);
            var cardUI = cardGO.GetComponent<TalentCardUI>();
            cardUI.Initialize(card, onTalentSelected);
        }
    }
}
