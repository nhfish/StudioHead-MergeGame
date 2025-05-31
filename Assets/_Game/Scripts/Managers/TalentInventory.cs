using System.Collections.Generic;
using UnityEngine;

public class TalentInventory : MonoBehaviour
{
    [Header("Starting Talent Assets")]
    public List<TalentBaseData> starterTalents;

    [Header("Runtime Talent Collection")]
    public List<TalentCard> ownedTalentCards = new List<TalentCard>();

    void Awake()
    {
        GenerateCardsFromBaseData();
    }

   void GenerateCardsFromBaseData()
{
    foreach (var data in starterTalents)
    {
        if (data == null)
        {
            Debug.LogWarning("Null entry in starterTalents list — skipping.");
            continue;
        }

        var newCard = new TalentCard(data);
        ownedTalentCards.Add(newCard);
    }

    Debug.Log($" Generated {ownedTalentCards.Count} talent cards.");
}

    public List<TalentCard> GetAvailableCards()
    {
        return ownedTalentCards.FindAll(card => card.IsUsable);
    }
}
