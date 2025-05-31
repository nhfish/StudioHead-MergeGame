using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DepartmentItemLibrary", menuName = "StudioMerge/Department Item Library")]
public class DepartmentItemLibrary : ScriptableObject
{
    [System.Serializable]
    public class DepartmentVisual
    {
        public DepartmentType department;
        public Sprite icon;
        // Optional: public Color tierColor; public AudioClip sfx; etc
    }

    public List<DepartmentVisual> visuals = new();

    private Dictionary<DepartmentType, Sprite> iconLookup;

    public void Init()
    {
        iconLookup = new Dictionary<DepartmentType, Sprite>();

        foreach (var visual in visuals)
        {
            if (!iconLookup.ContainsKey(visual.department))
            {
                iconLookup.Add(visual.department, visual.icon);
            }
        }
    }

    public Sprite GetIcon(DepartmentType type)
    {
        if (iconLookup == null || iconLookup.Count == 0)
            Init();

        if (iconLookup.TryGetValue(type, out Sprite icon))
            return icon;

        Debug.LogWarning($" No icon found for {type}");
        return null;
    }
}
