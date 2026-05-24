using UnityEngine;

[CreateAssetMenu(fileName = "NewClimbSetup", menuName = "Climb/SetUpClimb")]
public class SetUpClimb : ScriptableObject
{
    public float climbSpeed;

    [Header("Positions")]
    public Vector3 topPosition;
    public Vector3 bottomPosition;

    [Header("Rotation")]
    public Vector3 climbRotationEuler;

    [Header("Y limits")]
    public float minY;
    public float maxY;

    [Header("Start Y threshold")]
    public float startYThreshold;
    public float startYTop;
    public float startYBottom;
}
