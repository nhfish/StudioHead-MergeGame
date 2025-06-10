using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]

public class MovieRecipe
{
    public TalentCard writer;
    public TalentCard director;
    public TalentCard actor;

    public int fanReward;
    public int moneyReward;

    public List<DepartmentItemData> submittedItems = new();

    public int dailiesPlayed;
    public float rewardMultiplier = 1.0f;
    public List<int> dailyScores = new();

    public int TotalItemTier =>
        submittedItems.Sum(item => (int)item.tier + 1); // Tier1 = 1 ... Tier10 = 10

    public bool HasAllRequiredDepartments()
    {
        var requiredDepts = new HashSet<DepartmentType>
        {
            DepartmentType.Camera,
            DepartmentType.Sound,
            DepartmentType.Production
        };

        return submittedItems.Select(i => i.department).ToHashSet().IsSupersetOf(requiredDepts);
    }

    public bool HasGenreSynergy()
    {
        if (writer == null || director == null || actor == null)
            return false;

        var genre = writer.baseData.genre;
        return (director.baseData.genre == genre) && (actor.baseData.genre == genre);
    }

    public int TalentCount =>
        new[] { writer, director, actor }.Count(t => t != null);
}
