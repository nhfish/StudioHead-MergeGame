using UnityEngine;

public enum TalentRole { Writer, Director, Actor }
public enum TalentRarity { AList, BList, CList, DList }
public enum MovieGenre {
    Action, Drama, Comedy, Fantasy, Horror, Mystery, Romance,
    Thriller, SciFi, Western, Documentary, Animation, Musical, Biography
}

[CreateAssetMenu(menuName = "Studio/Talent")]
public class TalentBaseData : ScriptableObject
{
    public string talentName;
    public TalentRole role;
    public TalentRarity rarity;
    public MovieGenre genre;
    public Sprite portrait;

    public int MaxUses => rarity switch
    {
        TalentRarity.AList => 2,
        TalentRarity.BList => 3,
        TalentRarity.CList => 4,
        TalentRarity.DList => 5,
        _ => 3
    };
}
