using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovieRecipe
{
    public TalentCard writer;
    public TalentCard director;
    public TalentCard actor;

    public List<DepartmentItemData> submittedItems = new();

    public int TotalItemTier =>
        submittedItems.Sum(item => (int)item.tier + 1); // Basic = 1, Pro = 2, Elite = 3

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
