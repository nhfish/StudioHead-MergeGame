using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StudioMerge/Movie Recipe")]
public class MovieRecipeData : ScriptableObject
{
    [Header("Recipe Info")]
    public string movieTitle;
    public Sprite poster;

    [Header("Required Departments")]
    public List<DepartmentType> requiredDepartments = new List<DepartmentType>() {
        DepartmentType.Camera, DepartmentType.Sound, DepartmentType.Production
    };

    [Header("Optional Boost Departments")]
    public List<DepartmentType> bonusDepartments = new List<DepartmentType>() {
        DepartmentType.Art, DepartmentType.Wardrobe, DepartmentType.Lights, DepartmentType.Crafty, DepartmentType.Locations
    };

    [Header("Talent Slots (13 allowed)")]
    public TalentCard writer;
    public TalentCard director;
    public TalentCard actor;

    [Header("Result")]
    public GenreType finalGenre;
    public int baseFanReward;
    public int baseMoneyReward;
    public int productionTimeSeconds; // e.g. 480 for 8 minutes

    [Header("Missing Optional Department Penalties")]
    [Range(0f, 1f)] public float optionalDepartmentMoneyPenalty = 0f;
    [Range(0f, 1f)] public float optionalDepartmentFanPenalty = 0f;
    [Range(0f, 1f)] public float optionalDepartmentTimePenalty = 0f;

    [Header("Scoring Modifiers")]
    public bool grantSynergyBonus;
    public bool allowPartialTalent;

    [Header("Synergy Bonus Settings")]
    public SynergyBonusConfig synergyBonusConfig;
}
