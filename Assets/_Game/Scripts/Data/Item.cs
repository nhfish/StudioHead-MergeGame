using UnityEngine;

[System.Serializable]
public class Item
{
    public string departmentName;
    public int tier;
    public Sprite visual;

    public Item() { }

    public Item(DepartmentItemData data)
    {
        if (data == null)
        {
            Debug.LogError("Cannot create Item from null DepartmentItemData");
            return;
        }

        departmentName = data.department.ToString();
        tier = (int)data.tier;
        visual = data.icon;
    }
}
