using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Header("Version Control")]
    public int saveVersion = 1;
    public DateTime lastSaveTime;
    
    [Header("Economy")]
    public Dictionary<CurrencyType, int> currency = new();
    
    [Header("Grid State")]
    public List<GridItemData> gridItems = new();
    
    [Header("Inventory")]
    public List<Item> overflowItems = new();
    public int overflowSlots;
    
    [Header("Talent")]
    public List<TalentCardData> talentInventory = new();
    
    [Header("Production")]
    public List<SaveMovieRecipeData> productionQueue = new();
    public SaveMovieRecipeData activeProduction;
    public float productionRemainingTime;
    public float productionTotalTime;
    
    [Header("Crate Systems")]
    public Dictionary<DepartmentType, int> departmentEras = new();
    public float universalCrateTimer;
    
    [Header("Fame System")]
    public int fameLevel;
    public float fameProgress;
    
    [Header("Dailies")]
    public Dictionary<string, int> dailiesAttempts = new();
    public List<int> dailyScores = new();
    
    public SaveData()
    {
        lastSaveTime = DateTime.Now;
    }
}

[Serializable]
public class GridItemData
{
    public Vector2Int gridPosition;
    public string itemId;
    public DepartmentType department;
    public DepartmentItemTier tier;
}

[Serializable]
public class TalentCardData
{
    public string talentId;
    public TalentRarity rarity;
    public GenreType genre;
    public bool isLocked;
    public int useCount;
}

[Serializable]
public class SaveMovieRecipeData
{
    public TalentCardData writer;
    public TalentCardData director;
    public TalentCardData actor;
    public List<GridItemData> submittedItems = new();
    public int moneyReward;
    public int fanReward;
    public float rewardMultiplier;
    public int dailiesPlayed;
    public List<int> dailyScores = new();
} 