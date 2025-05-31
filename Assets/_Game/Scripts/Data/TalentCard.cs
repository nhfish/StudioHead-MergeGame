using System;
using UnityEngine;

[System.Serializable]
public class TalentCard
{
    public TalentBaseData baseData;

    [SerializeField] private int usesRemaining;
    public bool IsLocked { get; private set; } = false;

    public TalentCard(TalentBaseData data)
{
    if (data == null)
    {
        Debug.LogError("Attempted to create a TalentCard with null TalentBaseData!");
        return;
    }

    baseData = data;
    usesRemaining = data.MaxUses;
}

    public int UsesRemaining => usesRemaining;

    public void Use()
    {
        if (usesRemaining > 0)
            usesRemaining--;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public bool IsUsable => usesRemaining > 0 && !IsLocked;
}
