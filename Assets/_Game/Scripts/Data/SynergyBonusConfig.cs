using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "StudioMerge/Synergy Bonus Config")]
public class SynergyBonusConfig : ScriptableObject
{
    [Tooltip("Bonus multipliers indexed by tier (1=Tier1). Values as decimal bonuses, e.g. 0.05 for 5%.")]
    public List<float> tierMultipliers = new List<float>
    {
        0.05f, 0.07f, 0.09f, 0.11f, 0.13f,
        0.15f, 0.17f, 0.18f, 0.19f, 0.20f
    };

    public float GetBonusForTier(int tier)
    {
        if (tierMultipliers == null || tierMultipliers.Count == 0)
            return 0f;
        int index = Mathf.Clamp(tier - 1, 0, tierMultipliers.Count - 1);
        return tierMultipliers[index];
    }
}
