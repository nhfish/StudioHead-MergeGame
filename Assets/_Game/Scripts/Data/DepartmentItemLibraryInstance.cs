using UnityEngine;

public static class DepartmentItemLibraryInstance
{
    private static DepartmentItemLibrary _instance;

    public static DepartmentItemLibrary Get
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<DepartmentItemLibrary>("DepartmentItemLibrary");
                _instance.Init();
            }

            return _instance;
        }
    }

    public static Sprite GetIcon(DepartmentType type) => Get.GetIcon(type);
}
