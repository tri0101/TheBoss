using UnityEngine;

[CreateAssetMenu(fileName = "NewRotationSetup", menuName = "Rotation/SetUpRotationClick")]
public class SetUpRotationClick : ScriptableObject
{
    [Header("Rotation khi nhấn")]
    public Vector3 fromRotationEuler; // rotation ban đầu
    public Vector3 toRotationEuler;   // rotation muốn xoay đến

    [Header("Tốc độ xoay")]
    public float rotationSpeed = 2f;

    [Header("Chọn trục xoay")]
    public bool rotateX = true;
    public bool rotateY = true;
    public bool rotateZ = true;
}
