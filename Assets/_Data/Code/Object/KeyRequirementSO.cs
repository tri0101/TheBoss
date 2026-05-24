using UnityEngine;

[CreateAssetMenu(fileName = "KeyRequirement", menuName = "Interaction/KeyRequirement", order = 1)]
public class KeyRequirementSO : ScriptableObject
{
    public string requiredKeyName;       // Tên object đang cầm (ví dụ: "Key Blue")
    public string scriptParentName;      // Tên script nằm ở cha (ví dụ: "DoorController")
}
