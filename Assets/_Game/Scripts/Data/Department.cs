using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Studio/Department")]
public class Department : ScriptableObject
{
    public DepartmentType type;
    public int currentEra;
    public int crateCostSoft;
    public int crateCostPremium;
    public Sprite crateVisual;

    [System.Serializable]
    public struct TierWeight
    {
        public int tier;
        public float weight;
    }

    public List<TierWeight> dropWeights = new();
}
